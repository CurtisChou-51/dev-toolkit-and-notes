# IIS Dump 網站記憶體問題 Troubleshooting 2

- 問題背景：一個部署在 IIS 上的 .NET 網站記憶體使用長期偏高，且 CPU 使用率也會飆高

## 問題分析

- 與上次狀況類似，應該也是記憶體使用偏高造成，直接分析 dump 檔

dump 分析部分指令與結果示意：
```
!dumpheap -stat

00007ffbb3f315a8    45207      6055978 System.Char[]
00007ffbb3f34970    11720      6281920 System.Globalization.CultureData
00007ffbb3f34448   119675      9574000 System.Collections.Hashtable
00007ffbb3f2a488   178812     10932248 System.String[]
00007ffbb3f07880    30495     12236840 System.Int64[]
00007ffbb3eef9f8    84028     12772256 System.Runtime.Remoting.ServerIdentity
00007ffbb3f30bb0   674118     16178832 System.Object
00007ffbb3f33300   183077     16971024 System.Int32[]
00007ffbb3f23a78   121274     36730272 System.Collections.Hashtable+bucket[]
00007ffbb3f35848    39730     66106554 System.Byte[]
00007ffbb3f307a0   553915     95374460 System.String
0000028b2a19c560    30541    497335218      Free
00007ffbb3f30c48   112575    498622784 System.Object[]


!dumpheap -mt 00007ffbb3f30c48 -min 1000

         Address               MT     Size
000002965bb5b030 00007ffbb3f30c48    65304     
000002965bb6b9a8 00007ffbb3f30c48    10200     
000002965bb6e1a0 00007ffbb3f30c48    16344     
000002965bb72198 00007ffbb3f30c48    10200     
000002965bb74e70 00007ffbb3f30c48    65304     
000002965bbe3e08 00007ffbb3f30c48    32664     
000002965bbebdc0 00007ffbb3f30c48    32664     
000002965bc3b710 00007ffbb3f30c48    10200     
000002965bc3df08 00007ffbb3f30c48    16344     
000002965bc41f00 00007ffbb3f30c48    32664     
000002965bc49eb8 00007ffbb3f30c48    65304     
000002965bd831b0 00007ffbb3f30c48    32664    
```

- 針對 System.Object[] 將 mt 逐一執行 !dumpheap，發現許多 pinned handle

```
0:000> !gcroot 000002965bb5b030
HandleTable:
    000002994ffe13f8 (pinned handle)
    -> 000002965bb5b030 System.Object[]
	
0:000> !gcroot 000002965bb6b9a8
HandleTable:
    0000029930a215f8 (pinned handle)
    -> 000002965bb6b9a8 System.Object[]
	
0:000> !gcroot 000002965bb6e1a0
HandleTable:
    0000029930ac11f8 (pinned handle)
    -> 000002965bb6e1a0 System.Object[]
	
0:000> !gcroot 000002965bb72198
HandleTable:
    0000029931f215f8 (pinned handle)
    -> 000002965bb72198 System.Object[]

0:000> !gcroot 000002965bb74e70
HandleTable:
    000002992cc715e0 (pinned handle)
    -> 000002965bb74e70 System.Object[]
```

- !gchandles -stat 查看統計

```
0:000> !gchandles -stat
Statistics:
              MT    Count    TotalSize Class Name
00007ffbb0e92ad8        1           24 System.Web.Hosting.PipelineRuntime
00007ffbafa606c8        1           24 Bid+BindingCookie
00007ffbb3f06ca0        1           32 System.Threading.RegisteredWaitHandle
00007ffbb3f437e0        1           48 System.Threading.ManualResetEvent
00007ffbb3f30cc8        1           48 System.SharedStatics
00007ffbb0ea14d0        1           48 System.Web.Util.StopListeningWaitHandle
00007ffbb2e886d0        1           56 System.Diagnostics.TraceSource
00007ffbb3f41470        2           64 System.Diagnostics.Tracing.EtwSession
00007ffbb3f1fd00        1           64 System.EventHandler`1[[Windows.Foundation.Diagnostics.TracingStatusChangedEventArgs, mscorlib]]
00007ffbb0ea96f0        1           64 System.Web.Hosting.PrincipalFunctionDelegate
00007ffbb0ea9638        1           64 System.Web.Hosting.RoleFunctionDelegate
00007ffbb0ea9580        1           64 System.Web.Hosting.DisposeFunctionDelegate
00007ffbb0ea94c8        1           64 System.Web.Hosting.ExecuteFunctionDelegate
00007ffbb0ea9410        1           64 System.Web.Hosting.AsyncDisconnectNotificationDelegate
00007ffbb0ea9358        1           64 System.Web.Hosting.AsyncCompletionDelegate
00007ffbafa60638        1           64 Bid+CtrlCB
00007ffbb3f14190        2           80 System.Threading._ThreadPoolWaitOrTimerCallback
00007ffbb3efd8e8        1           80 System.Threading.PinnableBufferCache
00007ffbb0e90ed8        1           80 System.Web.Hosting.ProcessHost
00007ffbb2e8cba0        3          120 System.Net.TimerThread+TimerQueue
00007ffb577e3008        2          128 Microsoft.ReportingServices.Rendering.ImageRenderer.FontPackage+FreeMemory
00007ffb577e2e70        2          128 Microsoft.ReportingServices.Rendering.ImageRenderer.FontPackage+ReAllocateMemory
00007ffb577e2cd8        2          128 Microsoft.ReportingServices.Rendering.ImageRenderer.FontPackage+AllocateMemory
00007ffbb2ee5c58        1          136 System.Net.AutoWebProxyScriptEngine
00007ffbb3f30ad8        1          160 System.ExecutionEngineException
00007ffbb3f30a60        1          160 System.StackOverflowException
00007ffbb3f309e8        1          160 System.OutOfMemoryException
00007ffbb3f30950        1          160 System.Exception
00007ffbb5166808        1          192 System.Threading.CdsSyncEtwBCLProvider
00007ffbb3f3ec18        1          192 System.Threading.Tasks.TplEtwProvider
00007ffbb3f0d788        1          192 System.Collections.Concurrent.CDSCollectionETWBCLProvider
00007ffbb3efda70        1          192 System.Threading.PinnableBufferCacheEventSource
00007ffbb2ee6710        1          192 System.PinnableBufferCacheEventSource
00007ffbaf734260        6          192 System.Drawing.Internal.GPStream
00007ffbb0ea9ea0        1          200 System.Web.AspNetEventSource
00007ffbb2e83ef8        4          288 System.Diagnostics.TraceSwitch
00007ffbac9ad950        9          288 System.Transactions.SafeIUnknown
00007ffbb3f30b50        2          320 System.Threading.ThreadAbortException
00007ffbb2ee66b0        4          320 System.PinnableBufferCache
00007ffbb2e8cac8        6          336 System.Net.Logging+NclTraceSource
00007ffbb2e84060        5          360 System.Diagnostics.BooleanSwitch
00007ffbb0e96ec8        2          384 System.Web.TelemetryEventSource
00007ffbb2ee2d28        7          504 System.Diagnostics.SourceSwitch
00007ffbb3f32458       10          560 System.RuntimeType
00007ffb56b31c00        8         1088 Oracle.ManagedDataAccess.Types.OracleRefCursor
00007ffb56b310a0        9         1584 OracleInternal.ServiceObjects.OracleDataReaderImpl
00007ffbb0e959f8      128         5120 <>f__AnonymousType0`3[[System.Collections.Hashtable, mscorlib],[System.Web.Caching.CacheExpires, System.Web],[System.Web.Caching.CacheUsage, System.Web]]
00007ffbb3f2d958      167         5344 System.Threading.Timer
00007ffbaf3b1380       72         5760 System.Runtime.Caching.MemoryCacheStore
00007ffbb0e95608      128        13312 System.Web.Caching.CacheSingle
00007ffbb0e99cf8      284        18176 System.Web.NativeFileChangeNotification
00007ffbb3f317e8      250        24000 System.Threading.Thread
00007ffbb3f30bb0     3904        93696 System.Object
00007ffbb3f418f0     1875       120000 Microsoft.Win32.UnsafeNativeMethods+ManifestEtw+EtwEnableCallback
00007ffbb3ef65f0     2018       145296 System.Reflection.Emit.DynamicResolver
00007ffbb3f2f878     1093       174880 System.RuntimeType+RuntimeTypeCache
00007ffbb3f23f48     1866       358272 System.Diagnostics.Tracing.FrameworkEventSource
00007ffbb3f11510     3903       405912 System.Runtime.Remoting.Contexts.Context
00007ffbb3f31988     7804       499456 System.Security.PermissionSet
00007ffbb3f30d38     3903       843048 System.AppDomain
00007ffbb3f11008    10727       943976 System.Runtime.Remoting.Identity
00007ffbb3eef9f8    51631      7847912 System.Runtime.Remoting.ServerIdentity
00007ffbb3f30c48    20471    486666288 System.Object[]
Total 110337 objects

Handles:
    Strong Handles:       72665
    Pinned Handles:       15625
    Ref Count Handles:    9
    Weak Long Handles:    9044
    Weak Short Handles:   12830
    SizedRef Handles:     164
```

- 看起來是因為 pinned 太多導致問題。除了物件本身佔用的空間，因 GC 不能搬動 pinned 物件，大量的 Free 可能就是因此產生  
- `System.AppDomain` 的數量也異常，網站是 .NET framework 4.x，找了一些資料後發現可能是 LocalReport 未釋放造成

## 解決方案
- LocalReport 物件需要正確 `Dispose` (原先只有對 ReportViewer 物件做 `Dispose`，看起來還不夠)，再加上 `ReleaseSandboxAppDomain` 確保 AppDomain 被釋放

## 補充說明
- LocalReport 最常見的欄位運算式 (如：`=Fields!Amount.Value`)，即使不使用到 Code 或是自訂 Assembly 也需要進行運算式編譯，因此舊版 ReportViewer 使用 AppDomain 作為運算式編譯的隔離環境
- 新版的 LocalReport 改為使用 Roslyn 編譯器，並且不再使用 AppDomain 來隔離運算式編譯