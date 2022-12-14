using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using Data.Business;
using Data.DB;

namespace APIProject.Job
{
    public class Jobclass : IJob
    {
        public BookingBusiness bookBus ;
        protected WE_SHIPEntities Context;
        public WE_SHIPEntities GetContext()
        {
            if (Context == null)
            {
                Context = new WE_SHIPEntities();
            }
            return Context;
        }
        public Jobclass()
        {
            bookBus = new BookingBusiness(this.GetContext());
        }
        public async Task Execute(IJobExecutionContext context)
        {
            //SendRequestBusiness sendBus = new SendRequestBusiness();
            // sendBus.PushAfterWashing();
            bookBus.PushDriverRequestProcedure();
            bookBus.UpdateLocationDriverProcedure();
        }
    }
    /*
     * weship
1
latitude: 
18.0034858
longtitude: 
106.3898428
2
latitude: 
18.0676047
longtitude: 
106.3001275
3
latitude: 
18.067573
longtitude: 
106.3001596
4
latitude: 
18.0314638
longtitude: 
106.402412
5
latitude: 
18.034747782930577
longtitude: 
106.39348128939218
6
latitude: 
18.2030537
longtitude: 
106.1692077
8
latitude: 
21.0191512
longtitude: 
105.8085544
9
latitude: 
20.991224011716387
longtitude: 
105.63196928700617
10
latitude: 
20.985733563488935
longtitude: 
105.81168200679491
12
latitude: 
18.0033444
longtitude: 
106.3898346
13
latitude: 
18.3485518
longtitude: 
105.8978143
14
latitude: 
18.3499211060813
longtitude: 
105.89823213702016
15
latitude: 
18.350011482142868
longtitude: 
105.89817361854999
16
latitude: 
18.3749408
longtitude: 
105.8652073
17
latitude: 
18.3416431
longtitude: 
105.8929653
18
latitude: 
18.3448371
longtitude: 
105.889436
20
latitude: 
18.34135773307839
longtitude: 
105.91277875442992
21
latitude: 
18.350017399260466
longtitude: 
105.89817628333701
22
latitude: 
18.0663138
longtitude: 
106.3002946
23
latitude: 
18.341468542092155
longtitude: 
105.88192739092389
24
latitude: 
18.3502333
longtitude: 
105.9013554
25
latitude: 
18.3425057
longtitude: 
105.9025343
26
latitude: 
18.2204998
longtitude: 
105.9254385
27
latitude: 
18.341928441094552
longtitude: 
105.90295689190356
28
latitude: 
18.3223401
longtitude: 
105.9153657
30
latitude: 
21.0195391
longtitude: 
105.8088149
32
latitude: 
18.3456565
longtitude: 
105.8985831
33
latitude: 
18.5573467
longtitude: 
105.6952783
34
latitude: 
18.338357076339562
longtitude: 
105.89657116465501
35
latitude: 
18.339584112613586
longtitude: 
105.8947106700382
36
latitude: 
18.3377108
longtitude: 
105.8915351
37
latitude: 
18.3043487
longtitude: 
105.9037486
38
latitude: 
18.350158387812296
longtitude: 
105.90120784025389
39
latitude: 
18.3011807
longtitude: 
105.9263735
40
latitude: 
18.3502366
longtitude: 
105.9013522
41
latitude: 
18.329431419343134
longtitude: 
105.89797375340504
42
latitude: 
18.34980512843728
longtitude: 
105.90141995719016
43
latitude: 
18.33593176999999
longtitude: 
105.90218352999997
45
latitude: 
18.32450337997258
longtitude: 
105.91706307828089
46
latitude: 
18.3487105
longtitude: 
105.8860976
47
latitude: 
18.3291995
longtitude: 
105.8815499
48
latitude: 
18.344296654854446
longtitude: 
105.94709470917465
49
latitude: 
18.29960961290236
longtitude: 
105.93563782348099
50
latitude: 
18.337875873685178
longtitude: 
105.88428503325899
52
latitude: 
18.3226422
longtitude: 
105.8928206
53
latitude: 
18.337590705684168
longtitude: 
105.79155073922398
57
latitude: 
18.3918542
longtitude: 
105.8933209
60
latitude: 
18.33526203192272
longtitude: 
105.89367094384471
61
latitude: 
18.330447954595297
longtitude: 
105.90422935960704
63
latitude: 
18.3467679
longtitude: 
105.8868324
64
latitude: 
18.349843200772742
longtitude: 
105.90143892027568
65
latitude: 
18.3501914
longtitude: 
105.9013396
66
latitude: 
18.3502599
longtitude: 
105.9013004
67
latitude: 
18.343588735321017
longtitude: 
105.90743196159882
68
latitude: 
18.172324440832956
longtitude: 
106.14015824173777
69
latitude: 
18.344175364672946
longtitude: 
105.89782905202627
70
latitude: 
18.345334
longtitude: 
105.8749712
71
latitude: 
18.342520147738476
longtitude: 
105.89194724153046
72
latitude: 
18.2400364
longtitude: 
105.9314601
74
latitude: 
18.33931245067878
longtitude: 
105.89605950336959
76
latitude: 
18.3758993
longtitude: 
105.8439245
77
latitude: 
18.33701693328681
longtitude: 
105.87600761010982
78
latitude: 
18.3760713
longtitude: 
105.8438914
80
latitude: 
18.348520544642763
longtitude: 
105.89695031297134
81
latitude: 
18.334886223209118
longtitude: 
105.90379675184086
82
latitude: 
18.3356045
longtitude: 
105.9034913
83
latitude: 
18.350447551319
longtitude: 
105.90117639724187
84
latitude: 
18.36142
longtitude: 
105.9011074
85
latitude: 
18.3245819
longtitude: 
105.9167138
86
latitude: 
18.3267769
longtitude: 
105.8771413
87
latitude: 
18.3381352
longtitude: 
105.8985748
88
latitude: 
18.338219227365386
longtitude: 
105.89861558607804
89
latitude: 
18.342214233160938
longtitude: 
105.8974855069354
90
latitude: 
18.340763899207197
longtitude: 
105.90734945899287
91
latitude: 
18.35029993320491
longtitude: 
105.90127016913674
92
latitude: 
18.3242546
longtitude: 
105.913549
93
latitude: 
18.3349358
longtitude: 
105.9018707
94
latitude: 
18.3502406
longtitude: 
105.9013472
95
latitude: 
18.3475825
longtitude: 
105.9032925
96
latitude: 
18.4224823
longtitude: 
105.9356165
97
latitude: 
18.3348787316387
longtitude: 
105.89350445480558
98
latitude: 
18.337953
longtitude: 
105.9093553
99
latitude: 
18.2959505
longtitude: 
106.0251064
100
latitude: 
18.338219794924985
longtitude: 
105.89645908338336
105
latitude: 
18.0228394
longtitude: 
106.3918722
109
latitude: 
20.9855535
longtitude: 
105.811674
110
latitude: 
20.985553
longtitude: 
105.8116778
113
latitude: 
18.0227369
longtitude: 
106.3918588
115
latitude: 
20.9855593
longtitude: 
105.8116495
116
latitude: 
20.9855496
longtitude: 
105.8116724
117
latitude: 
20.985591254625778
longtitude: 
105.81174202693683
118
latitude: 
37.785834
longtitude: 
-122.406417
119
latitude: 
20.9866966
longtitude: 
105.8116009
120
latitude: 
20.985744507273846
longtitude: 
105.81168797074967
121
latitude: 
20.985751095104888
longtitude: 
105.8117084432901
122
latitude: 
18.0675827
longtitude: 
106.3002839
123
latitude: 
18.34770478775571
longtitude: 
105.89455042975572
124
latitude: 
18.3499055
longtitude: 
105.8983107
126
latitude: 
18.07444
longtitude: 
106.2900786
127
latitude: 
18.0228222
longtitude: 
106.3918726
128
latitude: 
18.0676449
longtitude: 
106.3001604
129
latitude: 
20.975845
longtitude: 
105.8173
130
latitude: 
20.985582925168874
longtitude: 
105.8117917497609
131
latitude: 
20.9858533
longtitude: 
105.8123217
134
latitude: 
20.9855636
longtitude: 
105.8116828
135
latitude: 
20.9758453
longtitude: 
105.8173
136
latitude: 
18.078652
longtitude: 
106.2903737
137
latitude: 
37.4219983
longtitude: 
-122.084
143
latitude: 
18.0660016
longtitude: 
106.298595
146
latitude: 
18.0668735
longtitude: 
106.3671299
147
latitude: 
21.019276359958553
longtitude: 
105.8084849493733
151
latitude: 
18.0630911
longtitude: 
106.3066924

     */
}