# clrstack

接續 gcroot 000001ccea1e1038
發現 Thread 88c8 不是 Finalizer Queue，故使用 !clrstack 查看

```
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

0:000> ~~[88c8]s
ntdll!NtWaitForMultipleObjects+0x14:
00007ffb`cab3f954 c3              ret
0:133> !clrstack
OS Thread Id: 0x88c8 (133)
        Child SP               IP Call Site
0000005653bfebf8 00007ffbcab3f954 [HelperMethodFrame_1OBJ: 0000005653bfebf8] System.Threading.WaitHandle.WaitMultiple(System.Threading.WaitHandle[], Int32, Boolean, Boolean)
0000005653bfed30 00007ffbb4467e90 System.Threading.WaitHandle.WaitAny(System.Threading.WaitHandle[], Int32, Boolean)
0000005653bfed90 00007ffbb310a336 System.Net.TimerThread.ThreadProc()
0000005653bfee40 00007ffbb4440bf8 System.Threading.ExecutionContext.RunInternal(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
0000005653bfef10 00007ffbb4440ae5 System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object, Boolean)
0000005653bfef40 00007ffbb4440ab5 System.Threading.ExecutionContext.Run(System.Threading.ExecutionContext, System.Threading.ContextCallback, System.Object)
0000005653bfef90 00007ffbb4465445 System.Threading.ThreadHelper.ThreadStart()
0000005653bff1b0 00007ffbb5d212c3 [GCFrame: 0000005653bff1b0] 
0000005653bff518 00007ffbb5d212c3 [DebuggerU2MCatchHandlerFrame: 0000005653bff518] 
0000005653bff698 00007ffbb5d212c3 [ContextTransitionFrame: 0000005653bff698] 
0000005653bff898 00007ffbb5d212c3 [DebuggerU2MCatchHandlerFrame: 0000005653bff898] 
```