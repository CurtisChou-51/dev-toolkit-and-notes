# gcroot

!dumpheap -mt 00007ffbb3f30c48 -min 1000
針對 System.Object[] 將 mt 逐一執行 !dumpheap，發現許多 pinned handle

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

!gchandles -type Pinned

```
          Handle Type                  Object     Size             Data Type
00000298419615f0 Pinned      00000294eb168c60    32664                  System.Object[]
00000298419615f8 Pinned      000002965b5360e0    10200                  System.Object[]
0000029841aa11f8 Pinned      000002965b5388d8    16344                  System.Object[]
0000029841ef15e0 Pinned      000002960affe548    65304                  System.Object[]
0000029841ef15e8 Pinned      000002960aff6590    32664                  System.Object[]
0000029841ef15f0 Pinned      000002960ae7ce78    16344                  System.Object[]
0000029841ef15f8 Pinned      000002960ae7a680    10200                  System.Object[]
000002983e9615e0 Pinned      000002950b191a10    65304                  System.Object[]
000002983e9615e8 Pinned      000002950b189a58    32664                  System.Object[]
000002983e9615f0 Pinned      000002950b185a60    16344                  System.Object[]
000002983e9615f8 Pinned      000002950b183268    10200                  System.Object[]
0000029840eb15e0 Pinned      00000295bb0ef908    65304                  System.Object[]
0000029840eb15e8 Pinned      00000295bb0d79e8    32664                  System.Object[]
0000029840eb15f0 Pinned      00000295bb0d39f0    16344                  System.Object[]
0000029840eb15f8 Pinned      00000295bb0c1570    10200                  System.Object[]
00000298417113f8 Pinned      00000294eb1582e0    65304                  System.Object[]
00000298417415e8 Pinned      00000294eb150328    32664                  System.Object[]
00000298417415f0 Pinned      000002951b10e248    16344                  System.Object[]
00000298417415f8 Pinned      000002951b10ba50    10200                  System.Object[]
00000298421813f8 Pinned      000002955b0f2ed8    65304                  System.Object[]
00000298421b15e8 Pinned      000002954b10b890    32664                  System.Object[]
```


!gchandles -stat 查看統計

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