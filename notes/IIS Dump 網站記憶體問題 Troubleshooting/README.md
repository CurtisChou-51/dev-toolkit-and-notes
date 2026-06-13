# IIS Dump 網站記憶體問題 Troubleshooting

- 問題背景：一個部署在 IIS 上的 .NET 網站記憶體使用長期偏高，且 CPU 使用率也會飆高

## 問題分析

- 系統本身並沒有著重 CPU 的操作，判斷可能是因為記憶體使用偏高，頻繁 GC 導致 CPU 飆高，要由 dump 檔取得更詳細資訊

dump 分析部分指令與結果示意：
```
!dumpheap -stat

00007ffbb3f33300    94897      8382008 System.Int32[]
00007ffbb510d9e8      126      8475600 System.Decimal[]
00007ffbb2eeaf98   136445      8732480 System.Net.AsyncProtocolRequest
00007ffbafa61688   102690      9858240 System.Data.DataRow
00007ffbb3f30c48   148078     12278232 System.Object[]
00007ffbb2ee9dd0    68323     12571432 System.Net.Sockets.OverlappedAsyncResult
00007ffbb2eea898   136431     13097376 System.Net.BufferAsyncResult
00007ffbb5131570    64849     18676512 System.Collections.Generic.Dictionary`2+Entry[[System.Type, mscorlib],[System.AttributeUsageAttribute, mscorlib]][]
00007ffbb3f315a8    24551     22586628 System.Char[]
00007ffbb3f307a0   370981     34668350 System.String
000001c2a95f74c0    40721    440449874      Free
00007ffbb3f35848    72003   2985899727 System.Byte[]


!dumpheap -mt 00007ffbb3f35848 -min 1000000

         Address               MT     Size
000001cbaa234e88 00007ffbb3f35848 203294953     
000001cbea246b80 00007ffbb3f35848 147122582     
000001ce08971020 00007ffbb3f35848 297356620     
000001cbfa1e1038 00007ffbb3f35848 241764109     
000001cc1a203750 00007ffbb3f35848 54228895     
000001ced3ef1020 00007ffbb3f35848 297356620     
000001ceb3ef1020 00007ffbb3f35848 297356620     
000001cc6a2c8350 00007ffbb3f35848 117888810     
000001cc71435ad8 00007ffbb3f35848 147122582     
000001cc7a26ea58 00007ffbb3f35848 42245985     
000001cc7cbb8a18 00007ffbb3f35848 147122582     
000001cc9a1e1038 00007ffbb3f35848 147122582     
000001ce5ca01020 00007ffbb3f35848 297356620     
000001ccea1e1038 00007ffbb3f35848 231811961     
000001ccfa25cf48 00007ffbb3f35848 231811961   

!gcroot 000001cbaa234e88
Finalizer Queue:
    000001c3aa269918
    -> 000001c3aa269918 System.Net.Connection
    -> 000001c8ea284a70 System.Net.TlsStream
    -> 000001c8ea284b18 System.Net.Security.SslState
    -> 000001c8ea284bf8 System.Net.Security.RemoteCertValidationCallback
    -> 000001c8ea284bd0 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001c6aa294728 System.Net.HttpWebRequest
    -> 000001c3aa269488 System.Net.ContextAwareResult
    -> 000001c6aa2946e8 System.Net.Http.HttpClientHandler+RequestState
    -> 000001c6aa294680 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c6aa294698 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c2aa28a7a8 System.Net.Http.HttpResponseMessage
    -> 000001c2aa28a818 System.Net.Http.StreamContent
    -> 000001c2aa28b2a8 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001cbaa234e88 System.Byte[]
```

- 分析 dump 檔，發現 `System.Byte[]` 佔據了大量的記憶體，之後使用 `gcroot` 追蹤，發現大量的 `System.Net` 命名空間下的物件，並且都在 Finalizer Queue

- 判斷可能因是該系統的 HttpClient 以及 HttpResponseMessage 等相關物件皆沒有被正確 Dispose，導致物件被送入 Finalizer Queue 等待被 GC 回收

> [!NOTE]
> Dispose 內除了釋放資源之外，還會會呼叫 GC.SuppressFinalize(this);
> 這會告訴 GC：「這個物件的資源已經釋放，把它的解構子註銷掉，不需要去解構子佇列排隊」