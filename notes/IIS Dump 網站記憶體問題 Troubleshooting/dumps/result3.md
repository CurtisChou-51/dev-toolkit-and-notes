# finalizequeue

接續 gcroot
發現有許多 Finalizer Queue，故使用 !finalizequeue 查看

```
0:000> !finalizequeue
SyncBlocks to be cleaned up: 0
Free-Threaded Interfaces to be released: 0
MTA Interfaces to be released: 0
STA Interfaces to be released: 0
----------------------------------
------------------------------
Heap 0
generation 0 has 595 finalizable objects (000001ce7ccee7d8->000001ce7ccefa70)
generation 1 has 9012 finalizable objects (000001ce7ccdce38->000001ce7ccee7d8)
generation 2 has 73 finalizable objects (000001ce7ccdcbf0->000001ce7ccdce38)
Ready for finalization 0 objects (000001ce7ccefa70->000001ce7ccefa70)
------------------------------
Heap 1
generation 0 has 269 finalizable objects (000001ce7cd9b3f8->000001ce7cd9bc60)
generation 1 has 12 finalizable objects (000001ce7cd9b398->000001ce7cd9b3f8)
generation 2 has 85 finalizable objects (000001ce7cd9b0f0->000001ce7cd9b398)
Ready for finalization 0 objects (000001ce7cd9bc60->000001ce7cd9bc60)
------------------------------
Heap 2
generation 0 has 561 finalizable objects (000001ce7d55e058->000001ce7d55f1e0)
generation 1 has 6 finalizable objects (000001ce7d55e028->000001ce7d55e058)
generation 2 has 77 finalizable objects (000001ce7d55ddc0->000001ce7d55e028)
Ready for finalization 0 objects (000001ce7d55f1e0->000001ce7d55f1e0)
------------------------------
Heap 3
generation 0 has 113 finalizable objects (000001cded08c800->000001cded08cb88)
generation 1 has 1 finalizable objects (000001cded08c7f8->000001cded08c800)
generation 2 has 73 finalizable objects (000001cded08c5b0->000001cded08c7f8)
Ready for finalization 0 objects (000001cded08cb88->000001cded08cb88)
------------------------------
Heap 4
generation 0 has 2626 finalizable objects (000001ce7d7cb970->000001ce7d7d0b80)
generation 1 has 0 finalizable objects (000001ce7d7cb970->000001ce7d7cb970)
generation 2 has 114 finalizable objects (000001ce7d7cb5e0->000001ce7d7cb970)
Ready for finalization 0 objects (000001ce7d7d0b80->000001ce7d7d0b80)
------------------------------
Heap 5
generation 0 has 973 finalizable objects (000001cded0b3150->000001cded0b4fb8)
generation 1 has 3 finalizable objects (000001cded0b3138->000001cded0b3150)
generation 2 has 51 finalizable objects (000001cded0b2fa0->000001cded0b3138)
Ready for finalization 0 objects (000001cded0b4fb8->000001cded0b4fb8)
------------------------------
Heap 6
generation 0 has 2724 finalizable objects (000001ce7d66c4a0->000001ce7d6719c0)
generation 1 has 0 finalizable objects (000001ce7d66c4a0->000001ce7d66c4a0)
generation 2 has 142 finalizable objects (000001ce7d66c030->000001ce7d66c4a0)
Ready for finalization 0 objects (000001ce7d6719c0->000001ce7d6719c0)
------------------------------
Heap 7
generation 0 has 1205 finalizable objects (000001ce7cc9b1d0->000001ce7cc9d778)
generation 1 has 2 finalizable objects (000001ce7cc9b1c0->000001ce7cc9b1d0)
generation 2 has 114 finalizable objects (000001ce7cc9ae30->000001ce7cc9b1c0)
Ready for finalization 0 objects (000001ce7cc9d778->000001ce7cc9d778)
------------------------------
Heap 8
generation 0 has 9716 finalizable objects (000001cded4fc898->000001cded50f838)
generation 1 has 0 finalizable objects (000001cded4fc898->000001cded4fc898)
generation 2 has 97 finalizable objects (000001cded4fc590->000001cded4fc898)
Ready for finalization 0 objects (000001cded50f838->000001cded50f838)
------------------------------
Heap 9
generation 0 has 3828 finalizable objects (000001c2a00329a8->000001c2a003a148)
generation 1 has 2 finalizable objects (000001c2a0032998->000001c2a00329a8)
generation 2 has 73 finalizable objects (000001c2a0032750->000001c2a0032998)
Ready for finalization 0 objects (000001c2a003a148->000001c2a003a148)
------------------------------
Heap 10
generation 0 has 2717 finalizable objects (000001c29ff3b860->000001c29ff40d48)
generation 1 has 27 finalizable objects (000001c29ff3b788->000001c29ff3b860)
generation 2 has 97 finalizable objects (000001c29ff3b480->000001c29ff3b788)
Ready for finalization 0 objects (000001c29ff40d48->000001c29ff40d48)
------------------------------
Heap 11
generation 0 has 2613 finalizable objects (000001c2a0091418->000001c2a00965c0)
generation 1 has 10 finalizable objects (000001c2a00913c8->000001c2a0091418)
generation 2 has 81 finalizable objects (000001c2a0091140->000001c2a00913c8)
Ready for finalization 0 objects (000001c2a00965c0->000001c2a00965c0)
------------------------------
Heap 12
generation 0 has 418 finalizable objects (000001c2a0058cb8->000001c2a00599c8)
generation 1 has 1 finalizable objects (000001c2a0058cb0->000001c2a0058cb8)
generation 2 has 112 finalizable objects (000001c2a0058930->000001c2a0058cb0)
Ready for finalization 0 objects (000001c2a00599c8->000001c2a00599c8)
------------------------------
Heap 13
generation 0 has 954 finalizable objects (000001c2a2033820->000001c2a20355f0)
generation 1 has 6 finalizable objects (000001c2a20337f0->000001c2a2033820)
generation 2 has 616 finalizable objects (000001c2a20324b0->000001c2a20337f0)
Ready for finalization 0 objects (000001c2a20355f0->000001c2a20355f0)
------------------------------
Heap 14
generation 0 has 630 finalizable objects (000001c2a1b41948->000001c2a1b42cf8)
generation 1 has 6 finalizable objects (000001c2a1b41918->000001c2a1b41948)
generation 2 has 91 finalizable objects (000001c2a1b41640->000001c2a1b41918)
Ready for finalization 0 objects (000001c2a1b42cf8->000001c2a1b42cf8)
------------------------------
Heap 15
generation 0 has 2201 finalizable objects (000001ce7d7a5618->000001ce7d7a9ae0)
generation 1 has 2 finalizable objects (000001ce7d7a5608->000001ce7d7a5618)
generation 2 has 65 finalizable objects (000001ce7d7a5400->000001ce7d7a5608)
Ready for finalization 0 objects (000001ce7d7a9ae0->000001ce7d7a9ae0)
------------------------------
Heap 16
generation 0 has 1746 finalizable objects (000001cdedc5c2b0->000001cdedc5f940)
generation 1 has 3 finalizable objects (000001cdedc5c298->000001cdedc5c2b0)
generation 2 has 89 finalizable objects (000001cdedc5bfd0->000001cdedc5c298)
Ready for finalization 0 objects (000001cdedc5f940->000001cdedc5f940)
------------------------------
Heap 17
generation 0 has 523 finalizable objects (000001ce7d78aed0->000001ce7d78bf28)
generation 1 has 0 finalizable objects (000001ce7d78aed0->000001ce7d78aed0)
generation 2 has 76 finalizable objects (000001ce7d78ac70->000001ce7d78aed0)
Ready for finalization 0 objects (000001ce7d78bf28->000001ce7d78bf28)
------------------------------
Heap 18
generation 0 has 442 finalizable objects (000001c29760fb68->000001c297610938)
generation 1 has 0 finalizable objects (000001c29760fb68->000001c29760fb68)
generation 2 has 131 finalizable objects (000001c29760f750->000001c29760fb68)
Ready for finalization 0 objects (000001c297610938->000001c297610938)
------------------------------
Heap 19
generation 0 has 3565 finalizable objects (000001ce7d1e2a38->000001ce7d1e99a0)
generation 1 has 1 finalizable objects (000001ce7d1e2a30->000001ce7d1e2a38)
generation 2 has 74 finalizable objects (000001ce7d1e27e0->000001ce7d1e2a30)
Ready for finalization 0 objects (000001ce7d1e99a0->000001ce7d1e99a0)
------------------------------
Heap 20
generation 0 has 2763 finalizable objects (000001ce7d57dc50->000001ce7d5832a8)
generation 1 has 2 finalizable objects (000001ce7d57dc40->000001ce7d57dc50)
generation 2 has 72 finalizable objects (000001ce7d57da00->000001ce7d57dc40)
Ready for finalization 0 objects (000001ce7d5832a8->000001ce7d5832a8)
------------------------------
Heap 21
generation 0 has 2771 finalizable objects (000001c29751f3a0->000001c297524a38)
generation 1 has 0 finalizable objects (000001c29751f3a0->000001c29751f3a0)
generation 2 has 68 finalizable objects (000001c29751f180->000001c29751f3a0)
Ready for finalization 0 objects (000001c297524a38->000001c297524a38)
------------------------------
Heap 22
generation 0 has 28985 finalizable objects (000001ce7d90b9e0->000001ce7d9443a8)
generation 1 has 3 finalizable objects (000001ce7d90b9c8->000001ce7d90b9e0)
generation 2 has 163 finalizable objects (000001ce7d90b4b0->000001ce7d90b9c8)
Ready for finalization 0 objects (000001ce7d9443a8->000001ce7d9443a8)
------------------------------
Heap 23
generation 0 has 1301 finalizable objects (000001ce7d805f78->000001ce7d808820)
generation 1 has 0 finalizable objects (000001ce7d805f78->000001ce7d805f78)
generation 2 has 63 finalizable objects (000001ce7d805d80->000001ce7d805f78)
Ready for finalization 0 objects (000001ce7d808820->000001ce7d808820)
------------------------------
Heap 24
generation 0 has 16487 finalizable objects (000001cded2b0cb8->000001cded2d0ff0)
generation 1 has 1 finalizable objects (000001cded2b0cb0->000001cded2b0cb8)
generation 2 has 114 finalizable objects (000001cded2b0920->000001cded2b0cb0)
Ready for finalization 0 objects (000001cded2d0ff0->000001cded2d0ff0)
------------------------------
Heap 25
generation 0 has 18567 finalizable objects (000001ce7d8d4af8->000001ce7d8f8f30)
generation 1 has 1 finalizable objects (000001ce7d8d4af0->000001ce7d8d4af8)
generation 2 has 140 finalizable objects (000001ce7d8d4690->000001ce7d8d4af0)
Ready for finalization 0 objects (000001ce7d8f8f30->000001ce7d8f8f30)
------------------------------
Heap 26
generation 0 has 4383 finalizable objects (000001c2a11b19b0->000001c2a11ba2a8)
generation 1 has 0 finalizable objects (000001c2a11b19b0->000001c2a11b19b0)
generation 2 has 166 finalizable objects (000001c2a11b1480->000001c2a11b19b0)
Ready for finalization 0 objects (000001c2a11ba2a8->000001c2a11ba2a8)
------------------------------
Heap 27
generation 0 has 5103 finalizable objects (000001c2974f1800->000001c2974fb778)
generation 1 has 1 finalizable objects (000001c2974f17f8->000001c2974f1800)
generation 2 has 73 finalizable objects (000001c2974f15b0->000001c2974f17f8)
Ready for finalization 0 objects (000001c2974fb778->000001c2974fb778)
------------------------------
Heap 28
generation 0 has 3936 finalizable objects (000001ce7cdd66c0->000001ce7cdde1c0)
generation 1 has 18 finalizable objects (000001ce7cdd6630->000001ce7cdd66c0)
generation 2 has 76 finalizable objects (000001ce7cdd63d0->000001ce7cdd6630)
Ready for finalization 0 objects (000001ce7cdde1c0->000001ce7cdde1c0)
------------------------------
Heap 29
generation 0 has 3890 finalizable objects (000001cdecf471c0->000001cdecf4eb50)
generation 1 has 1 finalizable objects (000001cdecf471b8->000001cdecf471c0)
generation 2 has 53 finalizable objects (000001cdecf47010->000001cdecf471b8)
Ready for finalization 0 objects (000001cdecf4eb50->000001cdecf4eb50)
------------------------------
Heap 30
generation 0 has 3789 finalizable objects (000001c2a2044e38->000001c2a204c4a0)
generation 1 has 1 finalizable objects (000001c2a2044e30->000001c2a2044e38)
generation 2 has 68 finalizable objects (000001c2a2044c10->000001c2a2044e30)
Ready for finalization 0 objects (000001c2a204c4a0->000001c2a204c4a0)
------------------------------
Heap 31
generation 0 has 2906 finalizable objects (000001c29ff78808->000001c29ff7e2d8)
generation 1 has 2 finalizable objects (000001c29ff787f8->000001c29ff78808)
generation 2 has 127 finalizable objects (000001c29ff78400->000001c29ff787f8)
Ready for finalization 0 objects (000001c29ff7e2d8->000001c29ff7e2d8)
------------------------------
Heap 32
generation 0 has 4683 finalizable objects (000001c2a205f910->000001c2a2068b68)
generation 1 has 2 finalizable objects (000001c2a205f900->000001c2a205f910)
generation 2 has 172 finalizable objects (000001c2a205f3a0->000001c2a205f900)
Ready for finalization 0 objects (000001c2a2068b68->000001c2a2068b68)
------------------------------
Heap 33
generation 0 has 1635 finalizable objects (000001ce7d840fd8->000001ce7d8442f0)
generation 1 has 0 finalizable objects (000001ce7d840fd8->000001ce7d840fd8)
generation 2 has 69 finalizable objects (000001ce7d840db0->000001ce7d840fd8)
Ready for finalization 0 objects (000001ce7d8442f0->000001ce7d8442f0)
------------------------------
Heap 34
generation 0 has 3229 finalizable objects (000001c2975a9690->000001c2975afb78)
generation 1 has 1 finalizable objects (000001c2975a9688->000001c2975a9690)
generation 2 has 99 finalizable objects (000001c2975a9370->000001c2975a9688)
Ready for finalization 0 objects (000001c2975afb78->000001c2975afb78)
------------------------------
Heap 35
generation 0 has 21450 finalizable objects (000001ce7d873810->000001ce7d89d660)
generation 1 has 0 finalizable objects (000001ce7d873810->000001ce7d873810)
generation 2 has 182 finalizable objects (000001ce7d873260->000001ce7d873810)
Ready for finalization 0 objects (000001ce7d89d660->000001ce7d89d660)
Statistics for all finalizable objects (including all objects ready for finalization):
              MT    Count    TotalSize Class Name
00007ffbb5181128        1           24 System.Reflection.Emit.DynamicResolver+DestroyScout
00007ffbb2ed9050        1           24 System.WeakReference`1[[System.Net.AutoWebProxyScriptEngine, System]]
00007ffbb0e9c0d0        1           24 System.Web.Configuration.ImpersonateTokenRef
00007ffbb515c0c8        1           32 System.Security.Cryptography.SafeCspHandle
00007ffbb3efdb20        1           32 System.Threading.Gen2GcCallback
00007ffbb2ee6200        1           32 System.Net.SafeInternetHandle
00007ffbb2ee5230        1           32 System.Net.SafeLocalFree
00007ffbb2ed77d8        1           32 System.IO.Compression.ZLibNative+SafeLibraryHandle
00007ffbb2ebc618        1           32 Microsoft.Win32.SafeHandles.SafeFileMapViewHandle
00007ffbb2eb4f58        1           32 Microsoft.Win32.SafeHandles.SafeFileMappingHandle
00007ffbb2e7e238        1           32 Microsoft.Win32.SafeHandles.SafeProcessHandle
00007ffbb0e997d8        1           32 System.Web.PerfInstanceDataHandle
00007ffbafa60710        1           32 Bid+AutoInit
00007ffb576624a8        1           32 Microsoft.ReportingServices.Rendering.RichText.Win32ObjectSafeHandle
00007ffb57610758        1           32 Microsoft.ReportingServices.Rendering.RichText.Win32DCSafeHandle
00007ffbb3f41168        1           40 System.LocalDataStoreSlot
00007ffbb3f14110        1           40 System.Threading.RegisteredWaitHandleSafe
00007ffbb2eeb548        1           40 System.Net.SafeCredentialReference
00007ffbb3f412c0        2           48 System.LocalDataStoreHolder
00007ffbb3f3a110        2           48 System.WeakReference`1[[System.Diagnostics.Tracing.EtwSession, mscorlib]]
00007ffbb2eeb168        1           48 System.Net.SafeFreeCredential_SECURITY
00007ffbb2e84c08        1           48 Microsoft.CSharp.CSharpCodeProvider
00007ffbac9a8728        1           48 System.Runtime.CompilerServices.ConditionalWeakTable`2[[System.Transactions.ContextKey, System.Transactions],[System.Transactions.ContextData, System.Transactions]]
00007ffbb0e9d358        1           56 System.Web.Compilation.CompilationMutex
00007ffbb0e9f978        2           80 System.Threading.ThreadLocal`1[[System.Collections.Concurrent.ConcurrentBag`1+ThreadLocalList[[System.Web.HttpApplication, System.Web]], System]]
00007ffbb3f0b5a0        4          128 Microsoft.Win32.SafeHandles.SafeLsaPolicyHandle
00007ffbb2ed1d98        4          128 System.Gen2GcCallback
00007ffbb3f1a618        4          160 Microsoft.Win32.SafeHandles.SafeLocalAllocHandle
00007ffbb2ee5f40        4          160 System.Net.SafeRegistryHandle
00007ffbb3f3ec18        1          192 System.Threading.Tasks.TplEtwProvider
00007ffbb3f0d788        1          192 System.Collections.Concurrent.CDSCollectionETWBCLProvider
00007ffbb3efda70        1          192 System.Threading.PinnableBufferCacheEventSource
00007ffbb2ee6710        1          192 System.PinnableBufferCacheEventSource
00007ffbb2ee4750        4          192 System.Net.SafeCloseSocketAndEvent
00007ffb56fcea88        2          192 Microsoft.Reporting.WebForms.ReportViewerStyle
00007ffb56aeb778        4          192 XXX.BizLayer.AuthManager
00007ffbb515aff8        5          200 System.Security.SafeBSTRHandle
00007ffbb0ea9ea0        1          200 System.Web.AspNetEventSource
00007ffbb2eeb7e8        8          256 System.Net.SafeFreeCertContext
00007ffbb2e82608        1          280 System.Diagnostics.Process
00007ffbaf7367f0        6          288 System.Drawing.Bitmap
00007ffbb3f0b630        8          320 Microsoft.Win32.SafeHandles.SafeLsaMemoryHandle
00007ffbb0ead898        4          320 System.Web.UI.WebControls.Style
00007ffb569abe50        4          352 OracleInternal.ConnectionPool.OraclePool
00007ffbb0e96ec8        2          384 System.Web.TelemetryEventSource
00007ffbb3f2b3a0       14          448 Microsoft.Win32.SafeHandles.SafeRegistryHandle
00007ffbb2e95288        1          488 System.Net.Sockets.Socket+TaskSocketAsyncEventArgs`1[[System.Net.Sockets.Socket, System]]
00007ffbb2e93e88        1          504 System.Net.Sockets.Socket+Int32TaskSocketAsyncEventArgs
00007ffbb2eec230       16          512 System.Net.SafeFreeCertChain
00007ffbb2eec5a0       13          520 System.Net.SafeFreeContextBufferChannelBinding_SECURITY
00007ffb569ab900        1          520 OracleInternal.ConnectionPool.OraclePoolManager
00007ffbb2eaa418       16          640 System.Net.Sockets.TcpClient
00007ffbb3f06328       24          768 System.Security.Cryptography.SafeHashHandle
00007ffb56b5c9a0       12          768 .
00007ffbb2eebea8       32         1024 System.Security.Cryptography.SafeLocalAllocHandle
00007ffbb2eeaea0       32         1024 System.Net.SafeFreeContextBuffer_SECURITY
00007ffbb2ecf6b8       32         1024 Microsoft.Win32.SafeHandles.SafeX509ChainHandle
00007ffbb2eeb3b0       21         1176 System.Net.SafeDeleteContext_SECURITY
00007ffbb5168760       38         1216 System.Threading.SafeCompressedStackHandle
00007ffbb2e93898        7         1232 System.Diagnostics.PerformanceCounter
00007ffbb3f41ea0       39         1248 System.Security.Cryptography.SafeProvHandle
00007ffbb3f16670       39         1248 Microsoft.Win32.SafeHandles.SafeFileMappingHandle
00007ffbb3f165e0       39         1248 Microsoft.Win32.SafeHandles.SafeViewOfFileHandle
00007ffbb3f0d130       39         1248 System.Threading.TimerQueue+AppDomainTimerSafeHandle
00007ffbb2eebab8       40         1280 System.Security.Cryptography.SafeCertStoreHandle
00007ffbb231a6f8       37         1480 Microsoft.Win32.SafeHandles.SafeCapiHashHandle
00007ffbac9ad950       48         1536 System.Transactions.SafeIUnknown
00007ffbb2ee45e0       51         1632 System.Net.SafeCloseSocket+InnerSafeCloseSocket
00007ffbb2eec428       21         1680 System.Net.Security._SslStream
00007ffbb3f05600       56         1792 System.Security.Cryptography.X509Certificates.SafeCertContextHandle
00007ffbb51562f0       38         1824 System.Runtime.CompilerServices.ConditionalWeakTable`2[[System.Object, mscorlib],[System.Runtime.Serialization.SerializationInfo, mscorlib]]
00007ffbb3f40038       57         1824 Microsoft.Win32.SafeHandles.SafeAccessTokenHandle
00007ffb56aed1e8       31         1984 XXX.BizLayer.RoleInfo
00007ffbb2ee47f8       62         2480 System.Net.SafeCloseSocket
00007ffbb2ee83b0       39         2496 System.Net.Sockets.NetworkStream
00007ffbb2eea4c8       21         2688 System.Net.TlsStream
00007ffbb2ee95b0       20         3040 System.Net.Sockets.ConnectOverlappedAsyncResult
00007ffbb0ee97d8       70         3360 System.Web.Security.FileSecurityDescriptorWrapper
00007ffbb0ef08a8       86         3440 System.Web.ClientImpersonationContext
00007ffbb0ea25f8      108         3456 System.Threading.ThreadLocal`1+FinalizationHelper[[System.Collections.Concurrent.ConcurrentBag`1+ThreadLocalList[[System.Web.HttpApplication, System.Web]], System]]
00007ffb56aebc38       31         3472 XXX.BizLayer.UserInfo
00007ffbb2eeb878      120         3840 System.Security.Cryptography.SafeCertContextHandle
00007ffbb0e99c58       54         3888 System.Web.DirMonCompletion
00007ffbb5154990      164         3936 System.SizedReference
00007ffbb3f41778       46         4048 System.Diagnostics.Tracing.EventSource+OverideEventProvider
00007ffbb3f11510       39         4056 System.Runtime.Remoting.Contexts.Context
00007ffbb3f0d058      244         5856 System.Threading.TimerHolder
00007ffbb2ee7e68       21         6720 System.Net.Connection
00007ffbb3f23f48       39         7488 System.Diagnostics.Tracing.FrameworkEventSource
00007ffbb2ee3010       62         8928 System.Net.Sockets.Socket
00007ffb56b03f90       71         9656 Oracle.ManagedDataAccess.Types.OracleRefCursor
00007ffbb3f156e0      242         9680 System.Threading.ThreadPoolWorkQueueThreadLocals
00007ffbb3f317e8      158        15168 System.Threading.Thread
00007ffbb231a568      562        17984 Microsoft.Win32.SafeHandles.SafeCspHandle
00007ffbb3ef65f0      274        19728 System.Reflection.Emit.DynamicResolver
00007ffbb231a698      545        21800 Microsoft.Win32.SafeHandles.SafeCapiKeyHandle
00007ffbb2e97ce8      424        23744 System.ComponentModel.Container
00007ffbb3f2e2f0      792        25344 Microsoft.Win32.SafeHandles.SafeWaitHandle
00007ffbb3f23e00      414        26496 System.Threading.ReaderWriterLock
00007ffbb2ee4888      840        26880 System.Net.SafeNativeOverlapped
00007ffb56b86158      354        28320 Oracle.ManagedDataAccess.Client.OracleLogicalTransaction
00007ffb56b8a270      102        30192 Oracle.ManagedDataAccess.Client.OracleDataReader
00007ffbb3f2d6d8     1415        33960 System.WeakReference
00007ffb56aed5f0      358        37232 XXX.BizLayer.FunInfo
00007ffbb0280cd8      222        39072 System.Data.DataSet
00007ffb56b8c738      217        39928 Oracle.ManagedDataAccess.Client.OracleDataAdapter
00007ffbb2ee9700      839        40272 System.Net.Sockets.OverlappedCache
00007ffbb0e968f0     1095        43800 System.Web.ApplicationImpersonationContext
00007ffb56b067d0      239        45888 Oracle.ManagedDataAccess.Client.OracleCommand
00007ffbb0e9a460     1642        65680 System.Web.ProcessImpersonationContext
00007ffbafa61488      286       146432 System.Data.DataTable
00007ffb56995b78      477       179352 Oracle.ManagedDataAccess.Client.OracleConnection
00007ffbafa61a70     1348       291168 System.Data.DataColumn
00007ffbb515c198    25320       810240 System.Security.Cryptography.SafeCspKeyHandle
00007ffbb515c130    25320       810240 System.Security.Cryptography.SafeCspHashHandle
00007ffbb0eb0160    43463      1738520 System.Web.HttpResponseUnmanagedBufferElement
00007ffbb2ee9dd0    68323     12571432 System.Net.Sockets.OverlappedAsyncResult
Total 177460 objects
```