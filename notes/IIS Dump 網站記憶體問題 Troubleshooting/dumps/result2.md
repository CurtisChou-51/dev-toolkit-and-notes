# gcroot

接續 !dumpheap -mt 00007ffbb3f35848 -min 1000000
將使用記憶體較多的 mt 逐一執行 !dumpheap

```
0:000> !gcroot 000001cbaa234e88
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

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001cbea246b80
Found 0 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001ce08971020
Finalizer Queue:
    000001c4ea20ff90
    -> 000001c4ea20ff90 System.Net.Connection
    -> 000001c5ea3f3158 System.Net.TlsStream
    -> 000001c5ea3f3200 System.Net.Security.SslState
    -> 000001c5ea3f32e0 System.Net.Security.RemoteCertValidationCallback
    -> 000001c5ea3f32b8 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001c56a2496a0 System.Net.HttpWebRequest
    -> 000001c4ea20fb00 System.Net.ContextAwareResult
    -> 000001c56a249660 System.Net.Http.HttpClientHandler+RequestState
    -> 000001c56a2495f8 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c56a249610 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c5ea3f4818 System.Net.Http.HttpResponseMessage
    -> 000001c5ea3f4888 System.Net.Http.StreamContent
    -> 000001c5ea3f5318 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001ce08971020 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001cbfa1e1038
Finalizer Queue:
    000001c2aa28ccc8
    -> 000001c2aa28ccc8 System.Net.TlsStream
    -> 000001c2aa28cd70 System.Net.Security.SslState
    -> 000001c2aa28ce50 System.Net.Security.RemoteCertValidationCallback
    -> 000001c2aa28ce28 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001c96a25d258 System.Net.HttpWebRequest
    -> 000001c76a1e5970 System.Net.ContextAwareResult
    -> 000001c96a25d218 System.Net.Http.HttpClientHandler+RequestState
    -> 000001c96a25d1b0 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c96a25d1c8 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c7ea283c20 System.Net.Http.HttpResponseMessage
    -> 000001c7ea283c90 System.Net.Http.StreamContent
    -> 000001c7ea284720 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001cbfa1e1038 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001cc1a203750
Finalizer Queue:
    000001c46a262590
    -> 000001c46a262590 System.Net.TlsStream
    -> 000001c46a262638 System.Net.Security.SslState
    -> 000001c46a262718 System.Net.Security.RemoteCertValidationCallback
    -> 000001c46a2626f0 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001ca2a2169a0 System.Net.HttpWebRequest
    -> 000001c9ea265db8 System.Net.ContextAwareResult
    -> 000001ca2a216960 System.Net.Http.HttpClientHandler+RequestState
    -> 000001ca2a2168f8 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001ca2a216910 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c46a263e48 System.Net.Http.HttpResponseMessage
    -> 000001c46a263eb8 System.Net.Http.StreamContent
    -> 000001c46a264948 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001cc1a203750 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001ced3ef1020
Thread 88c8:
*** WARNING: Unable to verify checksum for System.dll
    0000005653bfed90 00007ffbb310a336 System.Net.TimerThread.ThreadProc()
        r15:  (interior)
            ->  000001cc7a1f8420 System.Object[]
            ->  000001c5ea356408 System.Net.TimerThread+TimerQueue
            ->  000001c5ea356430 System.Net.TimerThread+TimerNode
            ->  000001c7aa78d328 System.Net.TimerThread+TimerNode
            ->  000001c56a38be60 System.Net.TimerThread+Callback
            ->  000001c56a38bb60 System.Net.ServicePoint
            ->  000001c56a38bc48 System.Collections.Hashtable
            ->  000001c56a38bc98 System.Collections.Hashtable+bucket[]
            ->  000001c8aaa4eac0 System.Net.ConnectionGroup
            ->  000001c66a8e0ea0 System.Net.TimerThread+TimerNode
            ->  000001c66a8e9bb8 System.Net.TimerThread+TimerNode
            ->  000001c66a8e9c00 System.Net.TimerThread+TimerNode
            ->  000001c66a9dab98 System.Net.TimerThread+TimerNode
            ->  000001c42a5b8e78 System.Net.TimerThread+Callback
            ->  000001c42a5b8b48 System.Net.ServicePoint
            ->  000001c42a5b8c30 System.Collections.Hashtable
            ->  000001c42a5b8c80 System.Collections.Hashtable+bucket[]
            ->  000001ca2a3174a0 System.Net.ConnectionGroup
            ->  000001c9ea6576f8 System.Net.TimerThread+TimerNode
            ->  000001c9ea6576b0 System.Net.TimerThread+TimerNode
            ->  000001c7aa974400 System.Net.TimerThread+TimerNode
            ->  000001c7ea392bb8 System.Net.ConnectionGroup
            ->  000001c7ea392c10 System.Collections.ArrayList
            ->  000001c7ea392c38 System.Object[]
            ->  000001c7ea392d30 System.Net.Connection
            ->  000001c66a9dafa8 System.Net.TlsStream
            ->  000001c66a9db050 System.Net.Security.SslState
            ->  000001c66a9db130 System.Net.Security.RemoteCertValidationCallback
            ->  000001c66a9db108 System.Net.ServicePoint+HandshakeDoneProcedure
            ->  000001ca2a3cc340 System.Net.HttpWebRequest
            ->  000001c7ea392360 System.Net.ContextAwareResult
            ->  000001ca2a3cc300 System.Net.Http.HttpClientHandler+RequestState
            ->  000001ca2a3cc298 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
            ->  000001ca2a3cc2b0 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
            ->  000001c3aa652fc0 System.Net.Http.HttpResponseMessage
            ->  000001c3aa653048 System.Net.Http.StreamContent
            ->  000001c3aa654110 System.Net.Http.HttpContent+LimitMemoryStream
            ->  000001ced3ef1020 System.Byte[]

Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001ceb3ef1020
Finalizer Queue:
    000001c56a24a810
    -> 000001c56a24a810 System.Net.Security._SslStream
    -> 000001c56a24a1f0 System.Net.Security.SslState
    -> 000001c56a24a2d0 System.Net.Security.RemoteCertValidationCallback
    -> 000001c56a24a2a8 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001ca2a213c00 System.Net.HttpWebRequest
    -> 000001c7ea282188 System.Net.ContextAwareResult
    -> 000001ca2a213bc0 System.Net.Http.HttpClientHandler+RequestState
    -> 000001ca2a213b58 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001ca2a213b70 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c56a24b808 System.Net.Http.HttpResponseMessage
    -> 000001c56a24b878 System.Net.Http.StreamContent
    -> 000001c56a24c308 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001ceb3ef1020 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001cc6a2c8350
Finalizer Queue:
    000001c8aa24ec98
    -> 000001c8aa24ec98 System.Net.TlsStream
    -> 000001c8aa24ed40 System.Net.Security.SslState
    -> 000001c8aa24ee20 System.Net.Security.RemoteCertValidationCallback
    -> 000001c8aa24edf8 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001c42a2e07d0 System.Net.HttpWebRequest
    -> 000001c9ea2667c0 System.Net.ContextAwareResult
    -> 000001c42a2e0790 System.Net.Http.HttpClientHandler+RequestState
    -> 000001c42a2e0728 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c42a2e0740 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c8aa2503a8 System.Net.Http.HttpResponseMessage
    -> 000001c8aa250418 System.Net.Http.StreamContent
    -> 000001c8aa250ea8 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001cc6a2c8350 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001cc71435ad8
Finalizer Queue:
    000001c2aa28f238
    -> 000001c2aa28f238 System.Net.Connection
    -> 000001c8aa264a58 System.Net.TlsStream
    -> 000001c8aa264b00 System.Net.Security.SslState
    -> 000001c8aa264be0 System.Net.Security.RemoteCertValidationCallback
    -> 000001c8aa264bb8 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001ca6a2ee4c8 System.Net.HttpWebRequest
    -> 000001c2aa28eda8 System.Net.ContextAwareResult
    -> 000001ca6a2ee488 System.Net.Http.HttpClientHandler+RequestState
    -> 000001ca6a2ee420 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001ca6a2ee438 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c6aa297ed0 System.Net.Http.HttpResponseMessage
    -> 000001c6aa297f40 System.Net.Http.StreamContent
    -> 000001c6aa2989d0 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001cc71435ad8 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001cc7a26ea58
Finalizer Queue:
    000001c62a29a0c8
    -> 000001c62a29a0c8 System.Net.Connection
    -> 000001c9ea267d58 System.Net.TlsStream
    -> 000001c9ea267e00 System.Net.Security.SslState
    -> 000001c9ea267ee0 System.Net.Security.RemoteCertValidationCallback
    -> 000001c9ea267eb8 System.Net.ServicePoint+HandshakeDoneProcedure
    -> 000001c4ea2134a8 System.Net.HttpWebRequest
    -> 000001c62a299c38 System.Net.ContextAwareResult
    -> 000001c4ea213468 System.Net.Http.HttpClientHandler+RequestState
    -> 000001c4ea213400 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c4ea213418 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
    -> 000001c5ea3f7080 System.Net.Http.HttpResponseMessage
    -> 000001c5ea3f70f0 System.Net.Http.StreamContent
    -> 000001c5ea402498 System.Net.Http.HttpContent+LimitMemoryStream
    -> 000001cc7a26ea58 System.Byte[]

Warning: These roots are from finalizable objects that are not yet ready for finalization.
This is to handle the case where objects re-register themselves for finalization.
These roots may be false positives.
Found 1 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot 000001ce5ca01020
Found 0 unique roots (run '!GCRoot -all' to see all roots).
0:000> !gcroot -all 000001ce5ca01020
Found 0 roots.
0:000> !gcroot 000001ccea1e1038
Thread 88c8:
    0000005653bfed90 00007ffbb310a336 System.Net.TimerThread.ThreadProc()
        r15:  (interior)
            ->  000001cc7a1f8420 System.Object[]
            ->  000001c5ea356408 System.Net.TimerThread+TimerQueue
            ->  000001c5ea356430 System.Net.TimerThread+TimerNode
            ->  000001c7aa78d328 System.Net.TimerThread+TimerNode
            ->  000001c56a38be60 System.Net.TimerThread+Callback
            ->  000001c56a38bb60 System.Net.ServicePoint
            ->  000001c56a38bc48 System.Collections.Hashtable
            ->  000001c56a38bc98 System.Collections.Hashtable+bucket[]
            ->  000001c8aaa4eac0 System.Net.ConnectionGroup
            ->  000001c66a8e0ea0 System.Net.TimerThread+TimerNode
            ->  000001c66a8e9bb8 System.Net.TimerThread+TimerNode
            ->  000001c66a8e9c00 System.Net.TimerThread+TimerNode
            ->  000001c66a9dab98 System.Net.TimerThread+TimerNode
            ->  000001c42a5b8e78 System.Net.TimerThread+Callback
            ->  000001c42a5b8b48 System.Net.ServicePoint
            ->  000001c42a5b8c30 System.Collections.Hashtable
            ->  000001c42a5b8c80 System.Collections.Hashtable+bucket[]
            ->  000001ca2a3174a0 System.Net.ConnectionGroup
            ->  000001ca2a3174f8 System.Collections.ArrayList
            ->  000001ca2a317520 System.Object[]
            ->  000001ca2a317618 System.Net.Connection
            ->  000001c7aa9748d0 System.Net.TlsStream
            ->  000001c7aa974978 System.Net.Security.SslState
            ->  000001c7aa974a58 System.Net.Security.RemoteCertValidationCallback
            ->  000001c7aa974a30 System.Net.ServicePoint+HandshakeDoneProcedure
            ->  000001c8aad0cd18 System.Net.HttpWebRequest
            ->  000001ca2a316c48 System.Net.ContextAwareResult
            ->  000001c8aad0ccd8 System.Net.Http.HttpClientHandler+RequestState
            ->  000001c8aad0cc70 System.Threading.Tasks.TaskCompletionSource`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
            ->  000001c8aad0cc88 System.Threading.Tasks.Task`1[[System.Net.Http.HttpResponseMessage, System.Net.Http]]
            ->  000001c7aa97b178 System.Net.Http.HttpResponseMessage
            ->  000001c7aa97b1e8 System.Net.Http.StreamContent
            ->  000001c7aa97c2c8 System.Net.Http.HttpContent+LimitMemoryStream
            ->  000001ccea1e1038 System.Byte[]

Found 1 unique roots (run '!GCRoot -all' to see all roots).
```
