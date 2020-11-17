# Protobuf and JSON comparison

|      Method |                 Categories | Items |          Mean |          Error |        StdDev | Ratio | RatioSD | Size in KB | Size ratio |
|------------ |--------------------------- |------ |--------------:|---------------:|--------------:|------:|--------:|-----------:|-----------:|
|    Protobuf |      Serialization,Singles |     1 |      72.10 us |      25.642 us |      1.406 us |  0.53 |    0.01 |          4 |       0.44 |
|        Json |      Serialization,Singles |     1 |     137.09 us |      31.440 us |      1.723 us |  1.00 |    0.00 |          9 |       1.00 |
| 'Json Gzip' |      Serialization,Singles |     1 |     347.75 us |     126.706 us |      6.945 us |  2.54 |    0.02 |          2 |       0.22 |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |   Serialization,Collection |     1 |      78.63 us |      17.003 us |      0.932 us |  0.50 |    0.01 |          4 |       0.44 |
|        Json |   Serialization,Collection |     1 |     155.76 us |       6.871 us |      0.377 us |  1.00 |    0.00 |          9 |       1.00 |
| 'Json Gzip' |   Serialization,Collection |     1 |     348.22 us |      73.799 us |      4.045 us |  2.24 |    0.02 |          2 |       0.22 |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |    Deserialization,Singles |     1 |      36.22 us |       2.281 us |      0.125 us |  0.27 |    0.00 |            |            |
|        Json |    Deserialization,Singles |     1 |     136.24 us |       7.403 us |      0.406 us |  1.00 |    0.00 |            |            |
| 'Json Gzip' |    Deserialization,Singles |     1 |     181.43 us |      36.398 us |      1.995 us |  1.33 |    0.02 |            |            |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf | Deserialization,Collection |     1 |      38.25 us |      20.912 us |      1.146 us |  0.29 |    0.01 |            |            |
|        Json | Deserialization,Collection |     1 |     132.14 us |      28.741 us |      1.575 us |  1.00 |    0.00 |            |            |
| 'Json Gzip' | Deserialization,Collection |     1 |     184.70 us |      65.378 us |      3.584 us |  1.40 |    0.02 |            |            |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |      Serialization,Singles |   100 |   5,352.16 us |   1,625.627 us |     89.106 us |  0.41 |    0.02 |        470 |       0.52 |
|        Json |      Serialization,Singles |   100 |  13,180.78 us |   6,422.721 us |    352.051 us |  1.00 |    0.00 |        908 |        1.0 |
| 'Json Gzip' |      Serialization,Singles |   100 |  32,915.44 us |  23,531.574 us |  1,289.845 us |  2.50 |    0.14 |        270 |       0.29 |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |   Serialization,Collection |   100 |   2,372.54 us |     388.918 us |     21.318 us |  0.26 |    0.00 |        470 |       0.52 |
|        Json |   Serialization,Collection |   100 |   9,038.36 us |   2,313.377 us |    126.804 us |  1.00 |    0.00 |        908 |        1.0 |
| 'Json Gzip' |   Serialization,Collection |   100 |  29,920.58 us |   8,751.165 us |    479.681 us |  3.31 |    0.10 |        240 |       0.26 |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |    Deserialization,Singles |   100 |   3,984.26 us |   2,175.485 us |    119.246 us |  0.28 |    0.01 |            |            |
|        Json |    Deserialization,Singles |   100 |  14,077.25 us |   3,452.724 us |    189.255 us |  1.00 |    0.00 |            |            |
| 'Json Gzip' |    Deserialization,Singles |   100 |  19,421.16 us |   7,756.111 us |    425.139 us |  1.38 |    0.03 |            |            |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf | Deserialization,Collection |   100 |   3,815.43 us |   4,739.887 us |    259.809 us |  0.24 |    0.02 |            |            |
|        Json | Deserialization,Collection |   100 |  15,722.65 us |   3,831.163 us |    209.999 us |  1.00 |    0.00 |            |            |
| 'Json Gzip' | Deserialization,Collection |   100 |  19,431.04 us |   8,430.603 us |    462.110 us |  1.24 |    0.03 |            |            |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |      Serialization,Singles |  1000 |  84,121.41 us |  21,948.527 us |  1,203.073 us |  0.52 |    0.01 |       4703 |       0.52 |
|        Json |      Serialization,Singles |  1000 | 162,638.17 us |  26,366.578 us |  1,445.241 us |  1.00 |    0.00 |       9085 |        1.0 |
| 'Json Gzip' |      Serialization,Singles |  1000 | 376,578.70 us | 416,042.863 us | 22,804.713 us |  2.32 |    0.14 |       2700 |       0.29 |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |   Serialization,Collection |  1000 |  26,209.49 us |   9,258.932 us |    507.513 us |  0.27 |    0.01 |       4706 |       0.52 |
|        Json |   Serialization,Collection |  1000 |  97,181.62 us |  11,487.792 us |    629.685 us |  1.00 |    0.00 |       9086 |        1.0 |
| 'Json Gzip' |   Serialization,Collection |  1000 | 272,288.53 us | 127,798.302 us |  7,005.056 us |  2.80 |    0.06 |       2403 |       0.26 |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf |    Deserialization,Singles |  1000 |  73,803.82 us |  20,336.096 us |  1,114.690 us |  0.46 |    0.00 |            |            |
|        Json |    Deserialization,Singles |  1000 | 161,347.60 us |  32,262.301 us |  1,768.406 us |  1.00 |    0.00 |            |            |
| 'Json Gzip' |    Deserialization,Singles |  1000 | 218,674.29 us |  59,415.729 us |  3,256.777 us |  1.36 |    0.03 |            |            |
|             |                            |       |               |                |               |       |         |            |            |
|    Protobuf | Deserialization,Collection |  1000 |  47,279.43 us |  26,684.037 us |  1,462.642 us |  0.29 |    0.01 |            |            |
|        Json | Deserialization,Collection |  1000 | 163,103.83 us |  19,700.907 us |  1,079.873 us |  1.00 |    0.00 |            |            |
| 'Json Gzip' | Deserialization,Collection |  1000 | 209,418.51 us |  45,808.311 us |  2,510.908 us |  1.28 |    0.01 |            |            |

## Legends

  Categories : All categories of the corresponded method, class, and assembly
  Items      : Value of the 'Items' parameter
  Mean       : Arithmetic mean of all measurements
  Error      : Half of 99.9% confidence interval
  StdDev     : Standard deviation of all measurements
  Ratio      : Mean of the ratio distribution ([Current]/[Baseline])
  RatioSD    : Standard deviation of the ratio distribution ([Current]/[Baseline])
  1 us       : 1 Microsecond (0.000001 sec)
  