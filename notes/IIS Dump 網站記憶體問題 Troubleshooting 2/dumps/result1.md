# dumpheap

列出記憶體使用狀況

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