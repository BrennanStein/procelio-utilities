// PROCELIO GAME ROTATION CODE
// Copyright Brennan Stein 2018
// Do not reuse or redistribute.


// Before you look further at this file, you must ask yourself:
//  "why am I here"
// There are many valid answers to that question. Curiosity, bugfixing, adding features, or so on.
// In the first case, look no further. As the saying goes, curiosity killed the cat.
// For your own sake, abandon this file at once. Were the current system of colliders and hitboxes
// to have critical bugs, the best choice may be to simply start over.

// This 3100+ line file will test the limits of your sanity. Over 80 different collider shapes, with
// metadata modifiers appended onto the front of the enums; custom rotation code for everything, and
// custom choicies and overrides for concavity.

// This treacherous path, wrought of hate in pursuit of efficiency, shall reveal its secrets only
// at personal cost. It is for your own benefit that I ask -- nay, beg -- that you spare yourself
// and leave maintainence of this file to someone who has already been corrupted by its influence.













using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotData
{

    public class ColPrivAlterationsConstants
    {
        public const int FORCEFLAT_PX = 1 << 26;
        public const int FORCECONVEX_PX = 2 << 26;
        public const int FORCECONCAVE_PX = 3 << 26;
        public const int FORCEFLAT_NX = 1 << 24;
        public const int FORCECONVEX_NX = 2 << 24;
        public const int FORCECONCAVE_NX = 3 << 24;
        public const int FORCEFLAT_PY = 1 << 22;
        public const int FORCECONVEX_PY = 2 << 22;
        public const int FORCECONCAVE_PY = 3 << 22;
        public const int FORCEFLAT_NY = 1 << 20;
        public const int FORCECONVEX_NY = 2 << 20;
        public const int FORCECONCAVE_NY = 3 << 20;
        public const int FORCEFLAT_PZ = 1 << 18;
        public const int FORCECONVEX_PZ = 2 << 18;
        public const int FORCECONCAVE_PZ = 3 << 18;
        public const int FORCEFLAT_NZ = 1 << 16;
        public const int FORCECONVEX_NZ = 2 << 16;
        public const int FORCECONCAVE_NZ = 3 << 16;
    }
    // XXXXXXXX YYYYYYYY ZZZZZZZZ WWWWWWWW
    //                            WWWWWWWW -- Collider primitive shape
    //                   ZZZZZZZZ -- Alteration shape data
    //     PXNX PYNYPZNZ          -- Flat/Convex/Concave OVERRIDES for sides
    // A___                       -- tag 1 if we ignore this for build mode
    public enum ColPrivAlterations
    {
        IGNORE_IN_BUILD_MODE = 1 << 31,

        FLAT = 0,
        CONCAVE = 1 << 8,
        CONVEX = 2 << 8,
        HIGH = 3 << 8,
        FULLSIDE = 4 << 8,
        PXSIDE2 = 1 << 12,
        PXSIDE1 = 2 << 12,
        PYSIDE2 = 3 << 12,
        PYSIDE1 = 4 << 12,
        PZSIDE2 = 5 << 12,
        PZSIDE1 = 6 << 12,

        ALTERATIONS_ONLY = (0xFF) << 8,
        BIT_FILTER_SIDE = (7) << 12,
        PXSIDE2_HIGH = PXSIDE2 | HIGH,
        PXSIDE1_HIGH = PXSIDE1 | HIGH,
        PYSIDE2_HIGH = PYSIDE2 | HIGH,
        PYSIDE1_HIGH = PYSIDE1 | HIGH,
        PZSIDE2_HIGH = PZSIDE2 | HIGH,
        PZSIDE1_HIGH = PZSIDE1 | HIGH,
        PXSIDE2_LOW_CONC = PXSIDE2 | CONCAVE,
        PXSIDE1_LOW_CONC = PXSIDE1 | CONCAVE,
        PYSIDE2_LOW_CONC = PYSIDE2 | CONCAVE,
        PYSIDE1_LOW_CONC = PYSIDE1 | CONCAVE,
        PZSIDE2_LOW_CONC = PZSIDE2 | CONCAVE,
        PZSIDE1_LOW_CONC = PZSIDE1 | CONCAVE,
        PXSIDE2_HIGH_CONC = PXSIDE2 | CONCAVE | HIGH,
        PXSIDE1_HIGH_CONC = PXSIDE1 | CONCAVE | HIGH,
        PYSIDE2_HIGH_CONC = PYSIDE2 | CONCAVE | HIGH,
        PYSIDE1_HIGH_CONC = PYSIDE1 | CONCAVE | HIGH,
        PZSIDE2_HIGH_CONC = PZSIDE2 | CONCAVE | HIGH,
        PZSIDE1_HIGH_CONC = PZSIDE1 | CONCAVE | HIGH,
        PXSIDE2_LOW_CONV = PXSIDE2 | CONVEX,
        PXSIDE1_LOW_CONV = PXSIDE1 | CONVEX,
        PYSIDE2_LOW_CONV = PYSIDE2 | CONVEX,
        PYSIDE1_LOW_CONV = PYSIDE1 | CONVEX,
        PZSIDE2_LOW_CONV = PZSIDE2 | CONVEX,
        PZSIDE1_LOW_CONV = PZSIDE1 | CONVEX,
        PXSIDE2_HIGH_CONV = PXSIDE2 | CONVEX | HIGH,
        PXSIDE1_HIGH_CONV = PXSIDE1 | CONVEX | HIGH,
        PYSIDE2_HIGH_CONV = PYSIDE2 | CONVEX | HIGH,
        PYSIDE1_HIGH_CONV = PYSIDE1 | CONVEX | HIGH,
        PZSIDE2_HIGH_CONV = PZSIDE2 | CONVEX | HIGH,
        PZSIDE1_HIGH_CONV = PZSIDE1 | CONVEX | HIGH,
    }
    public enum ColPriv
    {
        CUBE = 0,
        PX_NY_PRISM,
        NZ_NY_PRISM,
        NX_NY_PRISM,
        PZ_NY_PRISM,
        PX_NZ_PRISM,
        NX_NZ_PRISM,
        NX_PZ_PRISM,
        PX_PZ_PRISM,
        PX_PY_PRISM,
        NZ_PY_PRISM,
        NX_PY_PRISM,
        PZ_PY_PRISM,
        PX_PZ_NY_TETRA,
        PX_NZ_NY_TETRA,
        NX_NZ_NY_TETRA,
        NX_PZ_NY_TETRA,
        PX_PZ_PY_TETRA,
        PX_NZ_PY_TETRA,
        NX_NZ_PY_TETRA,
        NX_PZ_PY_TETRA,
        PX_PY_PZ_INNER,
        NX_PZ_PY_INNER,
        NX_NZ_PY_INNER,
        PX_NZ_PY_INNER,
        PX_PZ_NY_INNER,
        NX_PZ_NY_INNER,
        NX_NZ_NY_INNER,
        PX_NZ_NY_INNER,
        PX_SLAB,
        NX_SLAB,
        NY_SLAB,
        PY_SLAB,
        NZ_SLAB,
        PZ_SLAB,
        PX_NY_COVER,
        NZ_NY_COVER,
        NX_NY_COVER,
        PZ_NY_COVER,
        PX_NZ_COVER,
        NX_NZ_COVER,
        NX_PZ_COVER,
        PX_PZ_COVER,
        PX_PY_COVER,
        NZ_PY_COVER,
        NX_PY_COVER,
        PZ_PY_COVER,
        PX_PZ_NY_COVER,
        PX_NZ_NY_COVER,
        NX_NZ_NY_COVER,
        NX_PZ_NY_COVER,
        PX_PZ_PY_COVER,
        PX_NZ_PY_COVER,
        NX_NZ_PY_COVER,
        NX_PZ_PY_COVER,
        PX_PZ_NY_INCOV,
        PX_NZ_NY_INCOV,
        NX_NZ_NY_INCOV,
        NX_PZ_NY_INCOV,
        PX_PZ_PY_INCOV,
        NX_PZ_PY_INCOV,
        NX_NZ_PY_INCOV,
        PX_NZ_PY_INCOV,
        X_PILLAR,
        Z_PILLAR,
        Y_PILLAR,
        NX_NUB,
        PX_NUB,
        NZ_NUB,
        PZ_NUB,
        PY_NUB,
        NY_NUB,
        X_RIDGE_NY,
        Z_RIDGE_NY,
        X_RIDGE_PY,
        Z_RIDGE_PY,
        Y_RIDGE_PX,
        Z_RIDGE_PX,
        Y_RIDGE_NX,
        Z_RIDGE_NX,
        Y_RIDGE_NZ,
        X_RIDGE_NZ,
        Y_RIDGE_PZ,
        X_RIDGE_PZ,

    }
    public class CollideData
    {
        private const int NO_ALTERATION = 0xFF;
        private static HashSet<int> collisions = new HashSet<int> {
 267, 275, 276, 291, 316, 317, 322, 326, 331, 335, 524
, 529, 532, 548, 571, 572, 581, 582, 586, 595, 777, 785
, 786, 805, 827, 830, 835, 838, 843, 845, 1034, 1042
, 1043, 1062, 1085, 1086, 1092, 1094, 1098, 1105, 1287
, 1296, 1300, 1319, 1338, 1340, 1346, 1349, 1358, 1362
, 1544, 1549, 1553, 1576, 1591, 1595, 1603, 1605, 1612
, 1618, 1797, 1806, 1810, 1833, 1848, 1854, 1859, 1860
, 1868, 1872, 2054, 2063, 2067, 2090, 2105, 2109, 2114
, 2116, 2126, 2128, 2307, 2319, 2320, 2347, 2361, 2362
, 2370, 2375, 2377, 2383, 2564, 2573, 2576, 2604, 2615
, 2618, 2629, 2631, 2632, 2643, 2817, 2829, 2830, 2861
, 2871, 2872, 2883, 2887, 2889, 2893, 3074, 3086, 3087
, 3118, 3128, 3129, 3140, 3143, 3144, 3153, 3334, 3338
, 3339, 3343, 3346, 3347, 3348, 3353, 3363, 3366, 3370
, 3375, 3381, 3385, 3388, 3389, 3390, 3394, 3396, 3398
, 3402, 3403, 3406, 3407, 3408, 3409, 3591, 3595, 3596
, 3600, 3601, 3603, 3604, 3612, 3619, 3620, 3623, 3632
, 3638, 3642, 3643, 3644, 3645, 3650, 3653, 3654, 3658
, 3659, 3662, 3663, 3666, 3667, 3848, 3849, 3852, 3853
, 3857, 3858, 3860, 3867, 3876, 3877, 3880, 3889, 3891
, 3895, 3899, 3900, 3902, 3907, 3909, 3910, 3914, 3915
, 3916, 3917, 3922, 3923, 4101, 4105, 4106, 4110, 4113
, 4114, 4115, 4122, 4133, 4134, 4137, 4146, 4148, 4152
, 4155, 4157, 4158, 4163, 4164, 4166, 4170, 4171, 4172
, 4173, 4176, 4177, 4354, 4355, 4358, 4366, 4367, 4368
, 4371, 4373, 4394, 4395, 4398, 4401, 4403, 4408, 4409
, 4410, 4413, 4418, 4420, 4423, 4424, 4425, 4430, 4431
, 4432, 4433, 4611, 4612, 4615, 4621, 4623, 4624, 4628
, 4632, 4647, 4651, 4652, 4658, 4660, 4663, 4665, 4666
, 4668, 4674, 4677, 4679, 4680, 4681, 4686, 4687, 4690
, 4691, 4865, 4868, 4872, 4877, 4878, 4880, 4881, 4887
, 4904, 4908, 4909, 4911, 4917, 4919, 4920, 4922, 4923
, 4931, 4933, 4935, 4936, 4937, 4940, 4941, 4946, 4947
, 5121, 5122, 5125, 5133, 5134, 5135, 5138, 5142, 5161
, 5165, 5166, 5168, 5174, 5175, 5176, 5177, 5182, 5187
, 5188, 5191, 5192, 5193, 5196, 5197, 5200, 5201, 5393
, 5435, 5652, 5692, 5907, 5949, 6162, 6206, 6413, 6455
, 6672, 6714, 6927, 6969, 7182, 7224, 7454, 7490, 7502
, 7503, 7709, 7747, 7756, 7757, 7968, 8006, 8010, 8011
, 8223, 8263, 8264, 8265, 8482, 8517, 8530, 8531, 8737
, 8772, 8784, 8785, 8961, 8973, 8974, 9005, 9015, 9016
, 9027, 9031, 9033, 9037, 9218, 9230, 9231, 9262, 9272
, 9273, 9284, 9287, 9288, 9297, 9475, 9487, 9488, 9515
, 9529, 9530, 9538, 9543, 9545, 9551, 9732, 9741, 9744
, 9772, 9783, 9786, 9797, 9799, 9800, 9811, 9989, 9998
, 10002, 10025, 10040, 10046, 10051, 10052, 10060, 10064
, 10246, 10255, 10259, 10282, 10297, 10301, 10306, 10308
, 10318, 10320, 10503, 10512, 10516, 10535, 10554, 10556
, 10562, 10565, 10574, 10578, 10760, 10765, 10769, 10792
, 10807, 10811, 10819, 10821, 10828, 10834, 11017, 11025
, 11026, 11045, 11067, 11070, 11075, 11078, 11083, 11085
, 11274, 11282, 11283, 11302, 11325, 11326, 11332, 11334
, 11338, 11345, 11531, 11539, 11540, 11555, 11580, 11581
, 11586, 11590, 11595, 11599, 11788, 11793, 11796, 11812
, 11835, 11836, 11845, 11846, 11850, 11859, 12045, 12051
, 12085, 12087, 12093, 12098, 12100, 12102, 12106, 12107
, 12110, 12111, 12112, 12113, 12302, 12308, 12342, 12344
, 12348, 12354, 12357, 12358, 12362, 12363, 12366, 12367
, 12370, 12371, 12559, 12561, 12595, 12601, 12603, 12611
, 12613, 12614, 12618, 12619, 12620, 12621, 12626, 12627
, 12816, 12818, 12852, 12858, 12862, 12867, 12868, 12870
, 12874, 12875, 12876, 12877, 12880, 12881, 13071, 13073
, 13105, 13113, 13115, 13122, 13124, 13127, 13128, 13129
, 13134, 13135, 13136, 13137, 13328, 13330, 13362, 13370
, 13374, 13378, 13381, 13383, 13384, 13385, 13390, 13391
, 13394, 13395, 13581, 13587, 13615, 13623, 13629, 13635
, 13637, 13639, 13640, 13641, 13644, 13645, 13650, 13651
, 13838, 13844, 13872, 13880, 13884, 13891, 13892, 13895
, 13896, 13897, 13900, 13901, 13904, 13905, 14086, 14090
, 14091, 14095, 14098, 14099, 14100, 14105, 14115, 14118
, 14122, 14127, 14133, 14137, 14140, 14141, 14142, 14146
, 14148, 14150, 14154, 14155, 14158, 14159, 14160, 14161
, 14343, 14347, 14348, 14352, 14353, 14355, 14356, 14364
, 14371, 14372, 14375, 14384, 14390, 14394, 14395, 14396
, 14397, 14402, 14405, 14406, 14410, 14411, 14414, 14415
, 14418, 14419, 14600, 14601, 14604, 14605, 14609, 14610
, 14612, 14619, 14628, 14629, 14632, 14641, 14643, 14647
, 14651, 14652, 14654, 14659, 14661, 14662, 14666, 14667
, 14668, 14669, 14674, 14675, 14853, 14857, 14858, 14862
, 14865, 14866, 14867, 14874, 14885, 14886, 14889, 14898
, 14900, 14904, 14907, 14909, 14910, 14915, 14916, 14918
, 14922, 14923, 14924, 14925, 14928, 14929, 15106, 15107
, 15110, 15118, 15119, 15120, 15123, 15125, 15146, 15147
, 15150, 15153, 15155, 15160, 15161, 15162, 15165, 15170
, 15172, 15175, 15176, 15177, 15182, 15183, 15184, 15185
, 15361, 15362, 15365, 15373, 15374, 15375, 15378, 15382
, 15401, 15405, 15406, 15408, 15414, 15415, 15416, 15417
, 15422, 15427, 15428, 15431, 15432, 15433, 15436, 15437
, 15440, 15441, 15617, 15620, 15624, 15629, 15630, 15632
, 15633, 15639, 15656, 15660, 15661, 15663, 15669, 15671
, 15672, 15674, 15675, 15683, 15685, 15687, 15688, 15689
, 15692, 15693, 15698, 15699, 15875, 15876, 15879, 15885
, 15887, 15888, 15892, 15896, 15911, 15915, 15916, 15922
, 15924, 15927, 15929, 15930, 15932, 15938, 15941, 15943
, 15944, 15945, 15950, 15951, 15954, 15955, 16196, 16197
, 16198, 16199, 16200, 16201, 16202, 16203, 16208, 16209
, 16210, 16211, 16450, 16451, 16454, 16455, 16456, 16457
, 16458, 16459, 16460, 16461, 16462, 16463, 16706, 16707
, 16708, 16709, 16716, 16717, 16718, 16719, 16720, 16721
, 16722, 16723, 16897, 16901, 16904, 16905, 16909, 16910
, 16913, 16914, 16925, 16933, 16936, 16937, 16941, 16943
, 16944, 16947, 16948, 16951, 16952, 16955, 16958, 16960
, 16961, 16963, 16964, 16965, 16966, 16967, 16968, 16969
, 16970, 16971, 16972, 16973, 16976, 16977, 16978, 16979
, 17155, 17158, 17159, 17163, 17167, 17168, 17171, 17172
, 17182, 17187, 17191, 17194, 17195, 17201, 17202, 17205
, 17206, 17209, 17210, 17212, 17213, 17216, 17217, 17218
, 17220, 17221, 17222, 17223, 17224, 17225, 17226, 17227
, 17230, 17231, 17232, 17233, 17234, 17235, 17412, 17415
, 17416, 17420, 17421, 17424, 17425, 17428, 17442, 17444
, 17447, 17448, 17452, 17455, 17458, 17459, 17462, 17463
, 17466, 17467, 17468, 17471, 17473, 17474, 17475, 17477
, 17478, 17479, 17480, 17481, 17482, 17483, 17484, 17485
, 17486, 17487, 17490, 17491, 17666, 17669, 17670, 17674
, 17678, 17679, 17682, 17683, 17697, 17702, 17705, 17706
, 17710, 17712, 17713, 17716, 17717, 17720, 17721, 17725
, 17726, 17727, 17729, 17730, 17731, 17732, 17734, 17735
, 17736, 17737, 17738, 17739, 17740, 17741, 17742, 17743
, 17744, 17745, 17921, 17922, 17923, 17924, 17933, 17934
, 17935, 17936, 17951, 17963, 17964, 17965, 17966, 17967
, 17968, 17969, 17970, 17975, 17976, 17977, 17978, 17983
, 17984, 17986, 17987, 17988, 17989, 17991, 17992, 17993
, 17996, 17997, 17998, 17999, 18000, 18001, 18002, 18003
, 18185, 18186, 18187, 18188, 18193, 18194, 18195, 18196
, 18208, 18211, 18212, 18213, 18214, 18227, 18228, 18229
, 18230, 18235, 18236, 18237, 18238, 18239, 18240, 18242
, 18243, 18244, 18245, 18246, 18250, 18251, 18252, 18253
, 18254, 18255, 18256, 18257, 18258, 18259, 18442, 18444
, 18449, 18450, 18451, 18452, 18464, 18468, 18470, 18483
, 18484, 18485, 18486, 18491, 18492, 18493, 18494, 18495
, 18496, 18498, 18499, 18500, 18501, 18502, 18506, 18507
, 18509, 18511, 18512, 18513, 18514, 18515, 18697, 18699
, 18705, 18706, 18707, 18708, 18720, 18723, 18725, 18739
, 18740, 18741, 18742, 18747, 18748, 18749, 18750, 18751
, 18752, 18754, 18755, 18756, 18757, 18758, 18762, 18763
, 18764, 18765, 18766, 18767, 18769, 18771, 18946, 18948
, 18957, 18958, 18959, 18960, 18975, 18988, 18990, 18991
, 18992, 18993, 18994, 18999, 19000, 19001, 19002, 19007
, 19008, 19010, 19011, 19012, 19013, 19015, 19016, 19017
, 19021, 19023, 19024, 19025, 19026, 19027, 19201, 19203
, 19213, 19214, 19215, 19216, 19231, 19243, 19245, 19247
, 19248, 19249, 19250, 19255, 19256, 19257, 19258, 19263
, 19264, 19266, 19267, 19268, 19269, 19271, 19272, 19273
, 19276, 19277, 19278, 19279, 19281, 19283, 19462, 19463
, 19471, 19472, 19475, 19476, 19486, 19495, 19498, 19505
, 19506, 19509, 19510, 19513, 19514, 19516, 19517, 19520
, 19521, 19522, 19524, 19525, 19526, 19527, 19529, 19531
, 19534, 19535, 19536, 19537, 19538, 19539, 19715, 19723
, 19727, 19728, 19731, 19732, 19742, 19747, 19755, 19761
, 19762, 19765, 19766, 19769, 19770, 19772, 19773, 19776
, 19777, 19778, 19780, 19781, 19782, 19783, 19784, 19785
, 19786, 19787, 19790, 19791, 19792, 19794, 19973, 19976
, 19981, 19982, 19985, 19986, 19997, 20008, 20009, 20015
, 20016, 20019, 20020, 20023, 20024, 20027, 20030, 20032
, 20033, 20035, 20036, 20037, 20038, 20039, 20041, 20043
, 20044, 20045, 20048, 20049, 20050, 20051, 20225, 20233
, 20237, 20238, 20241, 20242, 20253, 20261, 20269, 20271
, 20272, 20275, 20276, 20279, 20280, 20283, 20286, 20288
, 20289, 20291, 20292, 20293, 20294, 20295, 20296, 20297
, 20298, 20299, 20300, 20301, 20304, 20306, 20487, 20488
, 20493, 20496, 20497, 20500, 20514, 20519, 20520, 20527
, 20530, 20531, 20534, 20535, 20538, 20539, 20540, 20543
, 20545, 20546, 20547, 20549, 20550, 20551, 20552, 20554
, 20556, 20557, 20558, 20559, 20562, 20563, 20740, 20748
, 20749, 20752, 20753, 20756, 20770, 20772, 20780, 20783
, 20786, 20787, 20790, 20791, 20794, 20795, 20796, 20799
, 20801, 20802, 20803, 20805, 20806, 20807, 20808, 20809
, 20810, 20811, 20812, 20814, 20818, 20819, 20997, 20998
, 21006, 21007, 21010, 21011, 21025, 21033, 21034, 21040
, 21041, 21044, 21045, 21048, 21049, 21053, 21054, 21055
, 21057, 21058, 21059, 21060, 21062, 21063, 21064, 21066
, 21068, 21069, 21070, 21071, 21072, 21073, 21250, 21258
, 21262, 21263, 21266, 21267, 21281, 21286, 21294, 21296
, 21297, 21300, 21301, 21304, 21305, 21309, 21310, 21311
, 21313, 21314, 21315, 21316, 21318, 21319, 21320, 21321
, 21322, 21323, 21324, 21326, 21328, 21329};

        public static bool BuildIgnore(ColPriv prim)
        {
            return ((int)prim & (int)ColPrivAlterations.IGNORE_IN_BUILD_MODE) != 0;
        }

        public static bool Collide(ColPriv c1, ColPriv c2)
        {
            return !(collisions.Contains(((NO_ALTERATION & (int)c1) << 8) | (NO_ALTERATION & (int)c2)));
        }

        public static ColPriv GetRotation(ColPriv c, byte rotation)
        {
            return (ColPriv)GetRotation((int)c, rotation);
        }

        public static int GetRotation(int c, byte rotation)
        {
            var alteration = (((int)c) & (~NO_ALTERATION));
            var side = alteration & (int)ColPrivAlterations.BIT_FILTER_SIDE;

            {

                int extrashape = (int)rotateoverridemetadata((ColPrivAlterations)(alteration & ~((int)ColPrivAlterations.BIT_FILTER_SIDE)),
                    (rotation >> 4) & 3, (rotation >> 2) & 3, rotation & 3);
                if (AlterationRotationData.ContainsKey(side))
                    side = AlterationRotationData[side](rotation);

                alteration = extrashape | side;
            }
            return ((int)alteration | (int)GetRotationSimple((ColPriv)((((int)c) & NO_ALTERATION)), rotation));
        }


        private static ColPriv GetRotationSimple(ColPriv c, byte rotation)
        {
            int xRot = (rotation & 0x30) >> 4;
            int yRot = (rotation & 0xC) >> 2;
            int zRot = (rotation & 0x3);
            switch (c)
            {
                case ColPriv.CUBE:
                    return ColPriv.CUBE;
                case ColPriv.PX_NY_PRISM:
                    return RotatePX_NY_PRISM(xRot, yRot, zRot);
                case ColPriv.NZ_NY_PRISM:
                    return RotateNZ_NY_PRISM(xRot, yRot, zRot);
                case ColPriv.NX_NY_PRISM:
                    return RotateNX_NY_PRISM(xRot, yRot, zRot);
                case ColPriv.PZ_NY_PRISM:
                    return RotatePZ_NY_PRISM(xRot, yRot, zRot);
                case ColPriv.PX_NZ_PRISM:
                    return RotatePX_NZ_PRISM(xRot, yRot, zRot);
                case ColPriv.NX_NZ_PRISM:
                    return RotateNX_NZ_PRISM(xRot, yRot, zRot);
                case ColPriv.NX_PZ_PRISM:
                    return RotateNX_PZ_PRISM(xRot, yRot, zRot);
                case ColPriv.PX_PZ_PRISM:
                    return RotatePX_PZ_PRISM(xRot, yRot, zRot);
                case ColPriv.PX_PY_PRISM:
                    return RotatePX_PY_PRISM(xRot, yRot, zRot);
                case ColPriv.NZ_PY_PRISM:
                    return RotateNZ_PY_PRISM(xRot, yRot, zRot);
                case ColPriv.NX_PY_PRISM:
                    return RotateNX_PY_PRISM(xRot, yRot, zRot);
                case ColPriv.PZ_PY_PRISM:
                    return RotatePZ_PY_PRISM(xRot, yRot, zRot);
                case ColPriv.PX_PZ_NY_TETRA:
                    return RotatePX_PZ_NY_TETRA(xRot, yRot, zRot);
                case ColPriv.PX_NZ_NY_TETRA:
                    return RotatePX_NZ_NY_TETRA(xRot, yRot, zRot);
                case ColPriv.NX_NZ_NY_TETRA:
                    return RotateNX_NZ_NY_TETRA(xRot, yRot, zRot);
                case ColPriv.NX_PZ_NY_TETRA:
                    return RotateNX_PZ_NY_TETRA(xRot, yRot, zRot);
                case ColPriv.PX_PZ_PY_TETRA:
                    return RotatePX_PZ_PY_TETRA(xRot, yRot, zRot);
                case ColPriv.PX_NZ_PY_TETRA:
                    return RotatePX_NZ_PY_TETRA(xRot, yRot, zRot);
                case ColPriv.NX_NZ_PY_TETRA:
                    return RotateNX_NZ_PY_TETRA(xRot, yRot, zRot);
                case ColPriv.NX_PZ_PY_TETRA:
                    return RotateNX_PZ_PY_TETRA(xRot, yRot, zRot);
                case ColPriv.PX_PY_PZ_INNER:
                    return RotatePX_PY_PZ_INNER(xRot, yRot, zRot);
                case ColPriv.NX_PZ_PY_INNER:
                    return RotateNX_PZ_PY_INNER(xRot, yRot, zRot);
                case ColPriv.NX_NZ_PY_INNER:
                    return RotateNX_NZ_PY_INNER(xRot, yRot, zRot);
                case ColPriv.PX_NZ_PY_INNER:
                    return RotatePX_NZ_PY_INNER(xRot, yRot, zRot);
                case ColPriv.PX_PZ_NY_INNER:
                    return RotatePX_PZ_NY_INNER(xRot, yRot, zRot);
                case ColPriv.NX_PZ_NY_INNER:
                    return RotateNX_PZ_NY_INNER(xRot, yRot, zRot);
                case ColPriv.NX_NZ_NY_INNER:
                    return RotateNX_NZ_NY_INNER(xRot, yRot, zRot);
                case ColPriv.PX_NZ_NY_INNER:
                    return RotatePX_NZ_NY_INNER(xRot, yRot, zRot);
                case ColPriv.PX_SLAB:
                    return RotatePX_SLAB(xRot, yRot, zRot);
                case ColPriv.NX_SLAB:
                    return RotateNX_SLAB(xRot, yRot, zRot);
                case ColPriv.NY_SLAB:
                    return RotateNY_SLAB(xRot, yRot, zRot);
                case ColPriv.PY_SLAB:
                    return RotatePY_SLAB(xRot, yRot, zRot);
                case ColPriv.NZ_SLAB:
                    return RotateNZ_SLAB(xRot, yRot, zRot);
                case ColPriv.PZ_SLAB:
                    return RotatePZ_SLAB(xRot, yRot, zRot);
                case ColPriv.PX_NY_COVER:
                    return RotatePX_NY_COVER(xRot, yRot, zRot);
                case ColPriv.NZ_NY_COVER:
                    return RotateNZ_NY_COVER(xRot, yRot, zRot);
                case ColPriv.NX_NY_COVER:
                    return RotateNX_NY_COVER(xRot, yRot, zRot);
                case ColPriv.PZ_NY_COVER:
                    return RotatePZ_NY_COVER(xRot, yRot, zRot);
                case ColPriv.PX_NZ_COVER:
                    return RotatePX_NZ_COVER(xRot, yRot, zRot);
                case ColPriv.NX_NZ_COVER:
                    return RotateNX_NZ_COVER(xRot, yRot, zRot);
                case ColPriv.NX_PZ_COVER:
                    return RotateNX_PZ_COVER(xRot, yRot, zRot);
                case ColPriv.PX_PZ_COVER:
                    return RotatePX_PZ_COVER(xRot, yRot, zRot);
                case ColPriv.PX_PY_COVER:
                    return RotatePX_PY_COVER(xRot, yRot, zRot);
                case ColPriv.NZ_PY_COVER:
                    return RotateNZ_PY_COVER(xRot, yRot, zRot);
                case ColPriv.NX_PY_COVER:
                    return RotateNX_PY_COVER(xRot, yRot, zRot);
                case ColPriv.PZ_PY_COVER:
                    return RotatePZ_PY_COVER(xRot, yRot, zRot);
                case ColPriv.PX_PZ_NY_COVER:
                    return RotatePX_PZ_NY_COVER(xRot, yRot, zRot);
                case ColPriv.PX_NZ_NY_COVER:
                    return RotatePX_NZ_NY_COVER(xRot, yRot, zRot);
                case ColPriv.NX_NZ_NY_COVER:
                    return RotateNX_NZ_NY_COVER(xRot, yRot, zRot);
                case ColPriv.NX_PZ_NY_COVER:
                    return RotateNX_PZ_NY_COVER(xRot, yRot, zRot);
                case ColPriv.PX_PZ_PY_COVER:
                    return RotatePX_PZ_PY_COVER(xRot, yRot, zRot);
                case ColPriv.PX_NZ_PY_COVER:
                    return RotatePX_NZ_PY_COVER(xRot, yRot, zRot);
                case ColPriv.NX_NZ_PY_COVER:
                    return RotateNX_NZ_PY_COVER(xRot, yRot, zRot);
                case ColPriv.NX_PZ_PY_COVER:
                    return RotateNX_PZ_PY_COVER(xRot, yRot, zRot);
                case ColPriv.PX_PZ_NY_INCOV:
                    return RotatePX_PZ_NY_INCOV(xRot, yRot, zRot);
                case ColPriv.PX_NZ_NY_INCOV:
                    return RotatePX_NZ_NY_INCOV(xRot, yRot, zRot);
                case ColPriv.NX_NZ_NY_INCOV:
                    return RotateNX_NZ_NY_INCOV(xRot, yRot, zRot);
                case ColPriv.NX_PZ_NY_INCOV:
                    return RotateNX_PZ_NY_INCOV(xRot, yRot, zRot);
                case ColPriv.PX_PZ_PY_INCOV:
                    return RotatePX_PZ_PY_INCOV(xRot, yRot, zRot);
                case ColPriv.NX_PZ_PY_INCOV:
                    return RotateNX_PZ_PY_INCOV(xRot, yRot, zRot);
                case ColPriv.NX_NZ_PY_INCOV:
                    return RotateNX_NZ_PY_INCOV(xRot, yRot, zRot);
                case ColPriv.PX_NZ_PY_INCOV:
                    return RotatePX_NZ_PY_INCOV(xRot, yRot, zRot);
                case ColPriv.X_PILLAR:
                    return RotateX_PILLAR(xRot, yRot, zRot);
                case ColPriv.Z_PILLAR:
                    return RotateZ_PILLAR(xRot, yRot, zRot);
                case ColPriv.Y_PILLAR:
                    return RotateY_PILLAR(xRot, yRot, zRot);
                case ColPriv.NX_NUB:
                    return RotateNX_NUB(xRot, yRot, zRot);
                case ColPriv.PX_NUB:
                    return RotatePX_NUB(xRot, yRot, zRot);
                case ColPriv.NZ_NUB:
                    return RotateNZ_NUB(xRot, yRot, zRot);
                case ColPriv.PZ_NUB:
                    return RotatePZ_NUB(xRot, yRot, zRot);
                case ColPriv.PY_NUB:
                    return RotatePY_NUB(xRot, yRot, zRot);
                case ColPriv.NY_NUB:
                    return RotateNY_NUB(xRot, yRot, zRot);
                case ColPriv.X_RIDGE_NY:
                    return RotateX_RIDGE_NY(xRot, yRot, zRot);
                case ColPriv.Z_RIDGE_NY:
                    return RotateZ_RIDGE_NY(xRot, yRot, zRot);
                case ColPriv.X_RIDGE_PY:
                    return RotateX_RIDGE_PY(xRot, yRot, zRot);
                case ColPriv.Z_RIDGE_PY:
                    return RotateZ_RIDGE_PY(xRot, yRot, zRot);
                case ColPriv.Y_RIDGE_PX:
                    return RotateY_RIDGE_PX(xRot, yRot, zRot);
                case ColPriv.Z_RIDGE_PX:
                    return RotateZ_RIDGE_PX(xRot, yRot, zRot);
                case ColPriv.Y_RIDGE_NX:
                    return RotateY_RIDGE_NX(xRot, yRot, zRot);
                case ColPriv.Z_RIDGE_NX:
                    return RotateZ_RIDGE_NX(xRot, yRot, zRot);
                case ColPriv.Y_RIDGE_NZ:
                    return RotateY_RIDGE_NZ(xRot, yRot, zRot);
                case ColPriv.X_RIDGE_NZ:
                    return RotateX_RIDGE_NZ(xRot, yRot, zRot);
                case ColPriv.Y_RIDGE_PZ:
                    return RotateY_RIDGE_PZ(xRot, yRot, zRot);
                case ColPriv.X_RIDGE_PZ:
                    return RotateX_RIDGE_PZ(xRot, yRot, zRot);
            }
            return c;
        }

        public static int GetShapeModifier(ColPriv col)
        {
            return (int)col & (~NO_ALTERATION);
        }
        public static int GetBaseShape(ColPriv col)
        {
            return (int)col & NO_ALTERATION;
        }

        public static Dictionary<int, EdgeRotate> AlterationRotationData = new Dictionary<int, EdgeRotate>
        {
            { (int)ColPrivAlterations.PXSIDE2, RotatePX2Type },
            { (int)ColPrivAlterations.PXSIDE1, RotatePX1Type },
            { (int)ColPrivAlterations.PYSIDE2, RotatePY2Type },
            { (int)ColPrivAlterations.PYSIDE1, RotatePY1Type },
            { (int)ColPrivAlterations.PZSIDE2, RotatePZ2Type },
            { (int)ColPrivAlterations.PZSIDE1, RotatePZ1Type }

        };

        private static int RotatePX1Type(int rot)
        {
            int x = rot >> 4;
            int y = (rot >> 2) & 3;
            int z = rot & 3;
            switch (z)
            {
                case 1:
                    return RotatePY1Type(x << 4 | y << 2);
                case 2:
                    return RotatePX2Type(x << 4 | y << 2);
                case 3:
                    return RotatePY2Type(x << 4 | y << 2);
            }

            // X doesn't change it
            switch (y)
            {
                case 1:
                    return RotatePZ2Type(0);
                case 2:
                    return RotatePX2Type(0);
                case 3:
                    return RotatePZ1Type(0);
            }

            return (int)ColPrivAlterations.PXSIDE1;
        }
        private static int RotatePX2Type(int rot)
        {
            int x = rot >> 4;
            int y = (rot >> 2) & 3;
            int z = rot & 3;
            switch (z)
            {
                case 1:
                    return RotatePY2Type(x << 4 | y << 2);
                case 2:
                    return RotatePX1Type(x << 4 | y << 2);
                case 3:
                    return RotatePY1Type(x << 4 | y << 2);
            }

            switch (y)
            {
                case 1:
                    return RotatePZ1Type(0);
                case 2:
                    return RotatePX1Type(0);
                case 3:
                    return RotatePZ2Type(0);
            }

            return (int)ColPrivAlterations.PXSIDE2;
        }
        private static int RotatePY1Type(int rot)
        {
            int x = rot >> 4;
            int y = (rot >> 2) & 3;
            int z = rot & 3;
            switch (z)
            {
                case 1:
                    return RotatePX2Type(x << 4 | y << 2);
                case 2:
                    return RotatePY2Type(x << 4 | y << 2);
                case 3:
                    return RotatePY1Type(x << 4 | y << 2);
            }

            switch (x)
            {
                case 1:
                    return RotatePZ1Type(y << 2);
                case 2:
                    return RotatePY2Type(y << 2);
                case 3:
                    return RotatePZ2Type(y << 2);
            }

            return (int)ColPrivAlterations.PYSIDE1;
        }
        private static int RotatePY2Type(int rot)
        {
            int x = rot >> 4;
            int y = (rot >> 2) & 3;
            int z = rot & 3;

            switch (z)
            {
                case 1:
                    return RotatePX1Type(x << 4 | y << 2);
                case 2:
                    return RotatePY1Type(x << 4 | y << 2);
                case 3:
                    return RotatePX2Type(x << 4 | y << 2);
            }

            switch (x)
            {
                case 1:
                    return RotatePZ2Type(y << 2);
                case 2:
                    return RotatePY1Type(y << 2);
                case 3:
                    return RotatePZ1Type(y << 2);
            }

            return (int)ColPrivAlterations.PYSIDE2;
        }
        private static int RotatePZ1Type(int rot)
        {
            int x = rot >> 4;
            int y = (rot >> 2) & 3;
            int z = rot & 3;

            switch (x)
            {
                case 1:
                    return RotatePY2Type(y << 2);
                case 2:
                    return RotatePZ2Type(y << 2);
                case 3:
                    return RotatePY1Type(y << 2);
            }
            switch (y)
            {
                case 1:
                    return RotatePX1Type(0);
                case 2:
                    return RotatePZ2Type(0);
                case 3:
                    return RotatePX2Type(0);
            }

            return (int)ColPrivAlterations.PZSIDE1;
        }
        private static int RotatePZ2Type(int rot)
        {
            int x = rot >> 4;
            int y = (rot >> 2) & 3;
            int z = rot & 3;
            switch (x)
            {
                case 1:
                    return RotatePY1Type(y << 2);
                case 2:
                    return RotatePZ1Type(y << 2);
                case 3:
                    return RotatePY2Type(y << 2);
            }
            switch (y)
            {
                case 1:
                    return RotatePX2Type(0);
                case 2:
                    return RotatePZ1Type(0);
                case 3:
                    return RotatePX1Type(0);
            }

            return (int)ColPrivAlterations.PZSIDE2;
        }

        // ARRAY ORDER:
        // +X, -X, +Y, -Y, +Z, -Z, SPECIAL (collide elsewhere)
        public static Dictionary<ColPriv, FaceCast[]> FaceCastData = new Dictionary<ColPriv, FaceCast[]>
        {
            {ColPriv.CUBE,       new FaceCast[7] { CastFacePX_SQUARE, CastFaceNX_SQUARE, CastFacePY_SQUARE, CastFaceNY_SQUARE, CastFacePZ_SQUARE, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.PX_NY_PRISM,  new FaceCast[7] { CastFacePX_SQUARE, CastFace_False, CastFace_False, CastFaceNY_SQUARE, CastFacePZ_TRI_PXNY, CastFaceNZ_TRI_PXNY, CastFace_False} },
            {ColPriv.NZ_NY_PRISM,  new FaceCast[7] { CastFacePX_TRI_NYNZ, CastFaceNX_TRI_NYNZ, CastFace_False, CastFaceNY_SQUARE, CastFace_False, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_NY_PRISM,  new FaceCast[7] { CastFace_False, CastFaceNX_SQUARE, CastFace_False, CastFaceNY_SQUARE, CastFacePZ_TRI_NXNY, CastFaceNZ_TRI_NXNY, CastFace_False} },
            {ColPriv.PZ_NY_PRISM,  new FaceCast[7] { CastFacePX_TRI_NYPZ, CastFaceNX_TRI_NYPZ, CastFace_False, CastFaceNY_SQUARE, CastFacePZ_SQUARE, CastFace_False, CastFace_False} },
            {ColPriv.PX_NZ_PRISM,  new FaceCast[7] { CastFacePX_SQUARE, CastFace_False, CastFacePY_TRI_PXNZ, CastFaceNY_TRI_PXNZ, CastFace_False, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_NZ_PRISM,  new FaceCast[7] { CastFace_False, CastFaceNX_SQUARE, CastFacePY_TRI_NXNZ, CastFaceNY_TRI_NXNZ, CastFace_False, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_PZ_PRISM,  new FaceCast[7] { CastFace_False, CastFaceNX_SQUARE, CastFacePY_TRI_NXPZ, CastFaceNY_TRI_NXPZ, CastFacePZ_SQUARE, CastFace_False, CastFace_False} },
            {ColPriv.PX_PZ_PRISM,  new FaceCast[7] { CastFacePX_SQUARE, CastFace_False, CastFacePY_TRI_PXPZ, CastFaceNY_TRI_PXPZ, CastFacePZ_SQUARE, CastFace_False, CastFace_False} },
            {ColPriv.PX_PY_PRISM,  new FaceCast[7] { CastFacePX_SQUARE, CastFace_False, CastFacePY_SQUARE, CastFace_False, CastFacePZ_TRI_PXPY, CastFaceNZ_TRI_PXPY, CastFace_False} },
            {ColPriv.NZ_PY_PRISM,  new FaceCast[7] { CastFacePX_TRI_PYNZ, CastFaceNX_TRI_PYNZ, CastFacePY_SQUARE, CastFace_False, CastFace_False, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_PY_PRISM,  new FaceCast[7] { CastFace_False, CastFaceNX_SQUARE, CastFacePY_SQUARE, CastFace_False, CastFacePZ_TRI_NXPY, CastFaceNZ_TRI_NXPY, CastFace_False} },
            {ColPriv.PZ_PY_PRISM,  new FaceCast[7] { CastFacePX_TRI_PYPZ, CastFaceNX_TRI_PYPZ, CastFacePY_SQUARE, CastFace_False, CastFacePZ_SQUARE, CastFace_False, CastFace_False} },
            {ColPriv.PX_PZ_NY_TETRA, new FaceCast[7] { CastFacePX_TRI_NYPZ, CastFace_False, CastFace_False, CastFaceNY_TRI_PXPZ, CastFacePZ_TRI_PXNY, CastFace_False, CastFace_False} },
            {ColPriv.PX_NZ_NY_TETRA, new FaceCast[7] { CastFacePX_TRI_NYNZ, CastFace_False, CastFace_False, CastFaceNY_TRI_PXNZ, CastFace_False, CastFaceNZ_TRI_PXNY, CastFace_False} },
            {ColPriv.NX_NZ_NY_TETRA, new FaceCast[7] { CastFace_False, CastFaceNX_TRI_NYNZ, CastFace_False, CastFaceNY_TRI_NXNZ, CastFace_False, CastFaceNZ_TRI_NXNY, CastFace_False} },
            {ColPriv.NX_PZ_NY_TETRA, new FaceCast[7] { CastFace_False, CastFaceNX_TRI_NYPZ, CastFace_False, CastFaceNY_TRI_NXPZ, CastFacePZ_TRI_NXNY, CastFace_False, CastFace_False} },
            {ColPriv.PX_PZ_PY_TETRA, new FaceCast[7] { CastFacePX_TRI_PYPZ, CastFace_False, CastFacePY_TRI_PXPZ, CastFace_False, CastFacePZ_TRI_PXPY, CastFace_False, CastFace_False} },
            {ColPriv.PX_NZ_PY_TETRA, new FaceCast[7] { CastFacePX_TRI_PYNZ, CastFace_False, CastFacePY_TRI_PXNZ, CastFace_False, CastFace_False, CastFaceNZ_TRI_PXPY, CastFace_False} },
            {ColPriv.NX_NZ_PY_TETRA, new FaceCast[7] { CastFace_False, CastFaceNX_TRI_PYNZ, CastFacePY_TRI_NXNZ, CastFace_False, CastFace_False, CastFaceNZ_TRI_NXPY, CastFace_False} },
            {ColPriv.NX_PZ_PY_TETRA, new FaceCast[7] { CastFace_False, CastFaceNX_TRI_PYPZ, CastFacePY_TRI_NXPZ, CastFace_False, CastFacePZ_TRI_NXPY, CastFace_False, CastFace_False} },
            {ColPriv.PX_PY_PZ_INNER, new FaceCast[7] { CastFacePX_TRI_NYNZ, CastFaceNX_SQUARE, CastFacePY_TRI_NXNZ, CastFaceNY_SQUARE, CastFacePZ_TRI_NXNY, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_PZ_PY_INNER, new FaceCast[7] { CastFacePX_SQUARE, CastFaceNX_TRI_NYNZ, CastFacePY_TRI_PXNZ, CastFaceNY_SQUARE, CastFacePZ_TRI_PXNY, CastFaceNY_SQUARE, CastFace_False} },
            {ColPriv.NX_NZ_PY_INNER, new FaceCast[7] { CastFacePX_SQUARE, CastFaceNX_TRI_NYPZ, CastFacePY_TRI_PXPZ, CastFaceNY_SQUARE, CastFacePZ_SQUARE, CastFaceNZ_TRI_PXNY, CastFace_False} },
            {ColPriv.PX_NZ_PY_INNER, new FaceCast[7] { CastFacePX_TRI_NYPZ, CastFaceNX_SQUARE, CastFacePY_TRI_NXPZ, CastFaceNY_SQUARE, CastFacePZ_SQUARE, CastFaceNZ_TRI_NXNY, CastFace_False} },
            {ColPriv.PX_PZ_NY_INNER, new FaceCast[7] { CastFacePX_TRI_PYNZ, CastFaceNX_SQUARE, CastFacePY_SQUARE, CastFaceNY_TRI_NXNZ, CastFacePZ_TRI_NXPY, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_PZ_NY_INNER, new FaceCast[7] { CastFacePX_SQUARE, CastFaceNX_TRI_PYNZ, CastFacePY_SQUARE, CastFaceNY_TRI_PXNZ, CastFacePZ_TRI_PXPY, CastFaceNZ_SQUARE, CastFace_False} },
            {ColPriv.NX_NZ_NY_INNER, new FaceCast[7] { CastFacePX_SQUARE, CastFaceNX_TRI_PYPZ, CastFacePY_SQUARE, CastFaceNY_TRI_PXPZ, CastFacePZ_SQUARE, CastFaceNZ_TRI_PXPY, CastFace_False} },
            {ColPriv.PX_NZ_NY_INNER, new FaceCast[7] { CastFacePX_TRI_PYPZ, CastFaceNX_SQUARE, CastFacePY_SQUARE, CastFaceNY_TRI_NXPZ, CastFacePZ_SQUARE, CastFaceNZ_TRI_NXPY, CastFace_False} }
        };


        private static ColPriv RotateCUBE(int x, int y, int z)
        {
            return ColPriv.CUBE;
        }
        private static ColPriv RotatePX_NY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PY_PRISM(x, y, 0);
                case 2: return RotateNX_PY_PRISM(x, y, 0);
                case 3: return RotateNX_NY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_PRISM(0, y, 0);
                case 2: return RotatePX_PY_PRISM(0, y, 0);
                case 3: return RotatePX_PZ_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NZ_NY_PRISM;
                case 2: return ColPriv.NX_NY_PRISM;
                case 3: return ColPriv.PZ_NY_PRISM;
            }
            return ColPriv.PX_NY_PRISM;
        }
        private static ColPriv RotateNZ_NY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_PRISM(x, y, 0);
                case 2: return RotateNZ_PY_PRISM(x, y, 0);
                case 3: return RotateNX_NZ_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNZ_PY_PRISM(0, y, 0);
                case 2: return RotatePZ_PY_PRISM(0, y, 0);
                case 3: return RotatePZ_NY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NY_PRISM;
                case 2: return ColPriv.PZ_NY_PRISM;
                case 3: return ColPriv.PX_NY_PRISM;
            }
            return ColPriv.NZ_NY_PRISM;
        }
        private static ColPriv RotateNX_NY_PRISM(int x, int y, int z)
        {
            Debug.Log(x + " " + y + " " + z);
            switch (z)
            {
                case 1: return RotatePX_NY_PRISM(x, y, 0);
                case 2: return RotatePX_PY_PRISM(x, y, 0);
                case 3: return RotateNX_PY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_PRISM(0, y, 0);
                case 2: return RotateNX_PY_PRISM(0, y, 0);
                case 3: return RotateNX_PZ_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PZ_NY_PRISM;
                case 2: return ColPriv.PX_NY_PRISM;
                case 3: return ColPriv.NZ_NY_PRISM;
            }
            return ColPriv.NX_NY_PRISM;
        }
        private static ColPriv RotatePZ_NY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_PRISM(x, y, 0);
                case 2: return RotatePZ_PY_PRISM(x, y, 0);
                case 3: return RotateNX_PZ_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNZ_NY_PRISM(0, y, 0);
                case 2: return RotateNZ_PY_PRISM(0, y, 0);
                case 3: return RotatePZ_PY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NY_PRISM;
                case 2: return ColPriv.NZ_NY_PRISM;
                case 3: return ColPriv.NX_NY_PRISM;
            }
            return ColPriv.PZ_NY_PRISM;
        }
        private static ColPriv RotatePX_NZ_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNZ_PY_PRISM(x, y, 0);
                case 2: return RotateNX_NZ_PRISM(x, y, 0);
                case 3: return RotateNZ_NY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PY_PRISM(0, y, 0);
                case 2: return RotatePX_PZ_PRISM(0, y, 0);
                case 3: return RotatePX_NY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_PRISM;
                case 2: return ColPriv.NX_PZ_PRISM;
                case 3: return ColPriv.PX_PZ_PRISM;
            }
            return ColPriv.PX_NZ_PRISM;
        }
        private static ColPriv RotateNX_NZ_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNZ_NY_PRISM(x, y, 0);
                case 2: return RotatePX_NZ_PRISM(x, y, 0);
                case 3: return RotateNZ_PY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PY_PRISM(0, y, 0);
                case 2: return RotateNX_PZ_PRISM(0, y, 0);
                case 3: return RotateNX_NY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_PRISM;
                case 2: return ColPriv.PX_PZ_PRISM;
                case 3: return ColPriv.PX_NZ_PRISM;
            }
            return ColPriv.NX_NZ_PRISM;
        }
        private static ColPriv RotateNX_PZ_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePZ_NY_PRISM(x, y, 0);
                case 2: return RotatePX_PZ_PRISM(x, y, 0);
                case 3: return RotatePZ_PY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NY_PRISM(0, y, 0);
                case 2: return RotateNX_NZ_PRISM(0, y, 0);
                case 3: return RotateNX_PY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_PRISM;
                case 2: return ColPriv.PX_NZ_PRISM;
                case 3: return ColPriv.NX_NZ_PRISM;
            }
            return ColPriv.NX_PZ_PRISM;
        }
        private static ColPriv RotatePX_PZ_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePZ_PY_PRISM(x, y, 0);
                case 2: return RotateNX_PZ_PRISM(x, y, 0);
                case 3: return RotatePZ_NY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NY_PRISM(0, y, 0);
                case 2: return RotatePX_NZ_PRISM(0, y, 0);
                case 3: return RotatePX_PY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_PRISM;
                case 2: return ColPriv.NX_NZ_PRISM;
                case 3: return ColPriv.NX_PZ_PRISM;
            }
            return ColPriv.PX_PZ_PRISM;
        }
        private static ColPriv RotatePX_PY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PY_PRISM(x, y, 0);
                case 2: return RotateNX_NY_PRISM(x, y, 0);
                case 3: return RotatePX_NY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_PRISM(0, y, 0);
                case 2: return RotatePX_NY_PRISM(0, y, 0);
                case 3: return RotatePX_NZ_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NZ_PY_PRISM;
                case 2: return ColPriv.NX_PY_PRISM;
                case 3: return ColPriv.PZ_PY_PRISM;
            }
            return ColPriv.PX_PY_PRISM;
        }
        private static ColPriv RotateNZ_PY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_PRISM(x, y, 0);
                case 2: return RotateNZ_NY_PRISM(x, y, 0);
                case 3: return RotatePX_NZ_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePZ_PY_PRISM(0, y, 0);
                case 2: return RotatePZ_NY_PRISM(0, y, 0);
                case 3: return RotateNZ_NY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PY_PRISM;
                case 2: return ColPriv.PZ_PY_PRISM;
                case 3: return ColPriv.PX_PY_PRISM;
            }
            return ColPriv.NZ_PY_PRISM;
        }
        private static ColPriv RotateNX_PY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NY_PRISM(x, y, 0);
                case 2: return RotatePX_NY_PRISM(x, y, 0);
                case 3: return RotatePX_PY_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_PRISM(0, y, 0);
                case 2: return RotateNX_NY_PRISM(0, y, 0);
                case 3: return RotateNX_NZ_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PZ_PY_PRISM;
                case 2: return ColPriv.PX_PY_PRISM;
                case 3: return ColPriv.NZ_PY_PRISM;
            }
            return ColPriv.NX_PY_PRISM;
        }
        private static ColPriv RotatePZ_PY_PRISM(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_PRISM(x, y, 0);
                case 2: return RotatePZ_NY_PRISM(x, y, 0);
                case 3: return RotatePX_PZ_PRISM(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePZ_NY_PRISM(0, y, 0);
                case 2: return RotateNZ_NY_PRISM(0, y, 0);
                case 3: return RotateNZ_PY_PRISM(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PY_PRISM;
                case 2: return ColPriv.NZ_PY_PRISM;
                case 3: return ColPriv.NX_PY_PRISM;
            }
            return ColPriv.PZ_PY_PRISM;
        }
        private static ColPriv RotatePX_PZ_NY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_PY_TETRA(x, y, 0);
                case 2: return RotateNX_PZ_PY_TETRA(x, y, 0);
                case 3: return RotateNX_PZ_NY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_NY_TETRA(0, y, 0);
                case 2: return RotatePX_NZ_PY_TETRA(0, y, 0);
                case 3: return RotatePX_PZ_PY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_NY_TETRA;
                case 2: return ColPriv.NX_NZ_NY_TETRA;
                case 3: return ColPriv.NX_PZ_NY_TETRA;
            }
            return ColPriv.PX_PZ_NY_TETRA;
        }
        private static ColPriv RotatePX_NZ_NY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_PY_TETRA(x, y, 0);
                case 2: return RotateNX_NZ_PY_TETRA(x, y, 0);
                case 3: return RotateNX_NZ_NY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_PY_TETRA(0, y, 0);
                case 2: return RotatePX_PZ_PY_TETRA(0, y, 0);
                case 3: return RotatePX_PZ_NY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_NY_TETRA;
                case 2: return ColPriv.NX_PZ_NY_TETRA;
                case 3: return ColPriv.PX_PZ_NY_TETRA;
            }
            return ColPriv.PX_NZ_NY_TETRA;
        }
        private static ColPriv RotateNX_NZ_NY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_NY_TETRA(x, y, 0);
                case 2: return RotatePX_NZ_PY_TETRA(x, y, 0);
                case 3: return RotateNX_NZ_PY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_PY_TETRA(0, y, 0);
                case 2: return RotateNX_PZ_PY_TETRA(0, y, 0);
                case 3: return RotateNX_PZ_NY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_NY_TETRA;
                case 2: return ColPriv.PX_PZ_NY_TETRA;
                case 3: return ColPriv.PX_NZ_NY_TETRA;
            }
            return ColPriv.NX_NZ_NY_TETRA;
        }
        private static ColPriv RotateNX_PZ_NY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_NY_TETRA(x, y, 0);
                case 2: return RotatePX_PZ_PY_TETRA(x, y, 0);
                case 3: return RotateNX_PZ_PY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_NY_TETRA(0, y, 0);
                case 2: return RotateNX_NZ_PY_TETRA(0, y, 0);
                case 3: return RotateNX_PZ_PY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_NY_TETRA;
                case 2: return ColPriv.PX_NZ_NY_TETRA;
                case 3: return ColPriv.NX_NZ_NY_TETRA;
            }
            return ColPriv.NX_PZ_NY_TETRA;
        }
        private static ColPriv RotatePX_PZ_PY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_PY_TETRA(x, y, 0);
                case 2: return RotateNX_PZ_NY_TETRA(x, y, 0);
                case 3: return RotatePX_PZ_NY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_NY_TETRA(0, y, 0);
                case 2: return RotatePX_NZ_NY_TETRA(0, y, 0);
                case 3: return RotatePX_NZ_PY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_PY_TETRA;
                case 2: return ColPriv.NX_NZ_PY_TETRA;
                case 3: return ColPriv.NX_PZ_PY_TETRA;
            }
            return ColPriv.PX_PZ_PY_TETRA;
        }
        private static ColPriv RotatePX_NZ_PY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_PY_TETRA(x, y, 0);
                case 2: return RotateNX_NZ_NY_TETRA(x, y, 0);
                case 3: return RotatePX_NZ_NY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_PY_TETRA(0, y, 0);
                case 2: return RotatePX_PZ_NY_TETRA(0, y, 0);
                case 3: return RotatePX_NZ_NY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_PY_TETRA;
                case 2: return ColPriv.NX_PZ_PY_TETRA;
                case 3: return ColPriv.PX_PZ_PY_TETRA;
            }
            return ColPriv.PX_NZ_PY_TETRA;
        }
        private static ColPriv RotateNX_NZ_PY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_NY_TETRA(x, y, 0);
                case 2: return RotatePX_NZ_NY_TETRA(x, y, 0);
                case 3: return RotatePX_NZ_PY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_PY_TETRA(0, y, 0);
                case 2: return RotateNX_PZ_NY_TETRA(0, y, 0);
                case 3: return RotateNX_NZ_NY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_PY_TETRA;
                case 2: return ColPriv.PX_PZ_PY_TETRA;
                case 3: return ColPriv.PX_NZ_PY_TETRA;
            }
            return ColPriv.NX_NZ_PY_TETRA;
        }
        private static ColPriv RotateNX_PZ_PY_TETRA(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_NY_TETRA(x, y, 0);
                case 2: return RotatePX_PZ_NY_TETRA(x, y, 0);
                case 3: return RotatePX_PZ_PY_TETRA(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_NY_TETRA(0, y, 0);
                case 2: return RotateNX_NZ_NY_TETRA(0, y, 0);
                case 3: return RotateNX_NZ_PY_TETRA(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_PY_TETRA;
                case 2: return ColPriv.PX_NZ_PY_TETRA;
                case 3: return ColPriv.NX_NZ_PY_TETRA;
            }
            return ColPriv.NX_PZ_PY_TETRA;
        }
        private static ColPriv RotatePX_PY_PZ_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_PY_INNER(x, y, 0);
                case 2: return RotateNX_PZ_NY_INNER(x, y, 0);
                case 3: return RotatePX_PZ_NY_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_NY_INNER(0, y, 0);
                case 2: return RotatePX_NZ_NY_INNER(0, y, 0);
                case 3: return RotatePX_NZ_PY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_PY_INNER;
                case 2: return ColPriv.NX_NZ_PY_INNER;
                case 3: return ColPriv.NX_PZ_PY_INNER;
            }
            return ColPriv.PX_PY_PZ_INNER;
        }
        private static ColPriv RotateNX_PZ_PY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_NY_INNER(x, y, 0);
                case 2: return RotatePX_PZ_NY_INNER(x, y, 0);
                case 3: return RotatePX_PY_PZ_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_NY_INNER(0, y, 0);
                case 2: return RotateNX_NZ_NY_INNER(0, y, 0);
                case 3: return RotateNX_NZ_PY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PY_PZ_INNER;
                case 2: return ColPriv.PX_NZ_PY_INNER;
                case 3: return ColPriv.NX_NZ_PY_INNER;
            }
            return ColPriv.NX_PZ_PY_INNER;
        }
        private static ColPriv RotateNX_NZ_PY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_NY_INNER(x, y, 0);
                case 2: return RotatePX_NZ_NY_INNER(x, y, 0);
                case 3: return RotatePX_NZ_PY_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_PY_INNER(0, y, 0);
                case 2: return RotateNX_PZ_NY_INNER(0, y, 0);
                case 3: return RotateNX_NZ_NY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_PY_INNER;
                case 2: return ColPriv.PX_PY_PZ_INNER;
                case 3: return ColPriv.PX_NZ_PY_INNER;
            }
            return ColPriv.NX_NZ_PY_INNER;
        }
        private static ColPriv RotatePX_NZ_PY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_PY_INNER(x, y, 0);
                case 2: return RotateNX_NZ_NY_INNER(x, y, 0);
                case 3: return RotatePX_PY_PZ_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PY_PZ_INNER(0, y, 0);
                case 2: return RotatePX_PZ_NY_INNER(0, y, 0);
                case 3: return RotatePX_NZ_NY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_PY_INNER;
                case 2: return ColPriv.NX_PZ_PY_INNER;
                case 3: return ColPriv.PX_PY_PZ_INNER;
            }
            return ColPriv.PX_NZ_PY_INNER;
        }
        private static ColPriv RotatePX_PZ_NY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PY_PZ_INNER(x, y, 0);
                case 2: return RotateNX_PZ_PY_INNER(x, y, 0);
                case 3: return RotateNX_PZ_NY_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_NY_INNER(0, y, 0);
                case 2: return RotatePX_NZ_PY_INNER(0, y, 0);
                case 3: return RotatePX_PY_PZ_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_NY_INNER;
                case 2: return ColPriv.NX_NZ_NY_INNER;
                case 3: return ColPriv.NX_PZ_NY_INNER;
            }
            return ColPriv.PX_PZ_NY_INNER;
        }
        private static ColPriv RotateNX_PZ_NY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_NY_INNER(x, y, 0);
                case 2: return RotatePX_PY_PZ_INNER(x, y, 0);
                case 3: return RotateNX_PZ_PY_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_NY_INNER(0, y, 0);
                case 2: return RotateNX_NZ_PY_INNER(0, y, 0);
                case 3: return RotateNX_PZ_PY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_NY_INNER;
                case 2: return ColPriv.PX_NZ_NY_INNER;
                case 3: return ColPriv.NX_NZ_NY_INNER;
            }
            return ColPriv.NX_PZ_NY_INNER;
        }
        private static ColPriv RotateNX_NZ_NY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_NY_INNER(x, y, 0);
                case 2: return RotatePX_NZ_PY_INNER(x, y, 0);
                case 3: return RotateNX_NZ_PY_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_PY_INNER(0, y, 0);
                case 2: return RotateNX_PZ_PY_INNER(0, y, 0);
                case 3: return RotateNX_PZ_NY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_NY_INNER;
                case 2: return ColPriv.PX_PZ_NY_INNER;
                case 3: return ColPriv.PX_NZ_NY_INNER;
            }
            return ColPriv.NX_NZ_NY_INNER;
        }
        private static ColPriv RotatePX_NZ_NY_INNER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_PY_INNER(x, y, 0);
                case 2: return RotateNX_NZ_PY_INNER(x, y, 0);
                case 3: return RotateNX_NZ_NY_INNER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_PY_INNER(0, y, 0);
                case 2: return RotatePX_PY_PZ_INNER(0, y, 0);
                case 3: return RotatePX_PZ_NY_INNER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_NY_INNER;
                case 2: return ColPriv.NX_PZ_NY_INNER;
                case 3: return ColPriv.PX_PZ_NY_INNER;
            }
            return ColPriv.PX_NZ_NY_INNER;
        }
        private static ColPriv RotatePX_SLAB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePY_SLAB(x, y, 0);
                case 2: return RotateNX_SLAB(x, y, 0);
                case 3: return RotateNY_SLAB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_SLAB(0, y, 0);
                case 2: return RotatePX_SLAB(0, y, 0);
                case 3: return RotatePX_SLAB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NZ_SLAB;
                case 2: return ColPriv.NX_SLAB;
                case 3: return ColPriv.PZ_SLAB;
            }
            return ColPriv.PX_SLAB;
        }
        private static ColPriv RotateNX_SLAB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNY_SLAB(x, y, 0);
                case 2: return RotatePX_SLAB(x, y, 0);
                case 3: return RotatePY_SLAB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_SLAB(0, y, 0);
                case 2: return RotateNX_SLAB(0, y, 0);
                case 3: return RotateNX_SLAB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PZ_SLAB;
                case 2: return ColPriv.PX_SLAB;
                case 3: return ColPriv.NZ_SLAB;
            }
            return ColPriv.NX_SLAB;
        }
        private static ColPriv RotateNY_SLAB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_SLAB(x, y, 0);
                case 2: return RotatePY_SLAB(x, y, 0);
                case 3: return RotateNX_SLAB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNZ_SLAB(0, y, 0);
                case 2: return RotatePY_SLAB(0, y, 0);
                case 3: return RotatePZ_SLAB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NY_SLAB;
                case 2: return ColPriv.NY_SLAB;
                case 3: return ColPriv.NY_SLAB;
            }
            return ColPriv.NY_SLAB;
        }
        private static ColPriv RotatePY_SLAB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_SLAB(x, y, 0);
                case 2: return RotateNY_SLAB(x, y, 0);
                case 3: return RotatePX_SLAB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePZ_SLAB(0, y, 0);
                case 2: return RotateNY_SLAB(0, y, 0);
                case 3: return RotateNZ_SLAB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PY_SLAB;
                case 2: return ColPriv.PY_SLAB;
                case 3: return ColPriv.PY_SLAB;
            }
            return ColPriv.PY_SLAB;
        }
        private static ColPriv RotateNZ_SLAB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNZ_SLAB(x, y, 0);
                case 2: return RotateNZ_SLAB(x, y, 0);
                case 3: return RotateNZ_SLAB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePY_SLAB(0, y, 0);
                case 2: return RotatePZ_SLAB(0, y, 0);
                case 3: return RotateNY_SLAB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_SLAB;
                case 2: return ColPriv.PZ_SLAB;
                case 3: return ColPriv.PX_SLAB;
            }
            return ColPriv.NZ_SLAB;
        }
        private static ColPriv RotatePZ_SLAB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePZ_SLAB(x, y, 0);
                case 2: return RotatePZ_SLAB(x, y, 0);
                case 3: return RotatePZ_SLAB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNY_SLAB(0, y, 0);
                case 2: return RotateNZ_SLAB(0, y, 0);
                case 3: return RotatePY_SLAB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_SLAB;
                case 2: return ColPriv.NZ_SLAB;
                case 3: return ColPriv.NX_SLAB;
            }
            return ColPriv.PZ_SLAB;
        }
        private static ColPriv RotatePX_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PY_COVER(x, y, 0);
                case 2: return RotateNX_PY_COVER(x, y, 0);
                case 3: return RotateNX_NY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_COVER(0, y, 0);
                case 2: return RotatePX_PY_COVER(0, y, 0);
                case 3: return RotatePX_PZ_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NZ_NY_COVER;
                case 2: return ColPriv.NX_NY_COVER;
                case 3: return ColPriv.PZ_NY_COVER;
            }
            return ColPriv.PX_NY_COVER;
        }
        private static ColPriv RotateNZ_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_COVER(x, y, 0);
                case 2: return RotateNZ_PY_COVER(x, y, 0);
                case 3: return RotateNX_NZ_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNZ_PY_COVER(0, y, 0);
                case 2: return RotatePZ_PY_COVER(0, y, 0);
                case 3: return RotatePZ_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NY_COVER;
                case 2: return ColPriv.PZ_NY_COVER;
                case 3: return ColPriv.PX_NY_COVER;
            }
            return ColPriv.NZ_NY_COVER;
        }
        private static ColPriv RotateNX_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NY_COVER(x, y, 0);
                case 2: return RotatePX_PY_COVER(x, y, 0);
                case 3: return RotateNX_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_COVER(0, y, 0);
                case 2: return RotateNX_PY_COVER(0, y, 0);
                case 3: return RotateNX_PZ_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PZ_NY_COVER;
                case 2: return ColPriv.PX_NY_COVER;
                case 3: return ColPriv.NZ_NY_COVER;
            }
            return ColPriv.NX_NY_COVER;
        }
        private static ColPriv RotatePZ_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_COVER(x, y, 0);
                case 2: return RotatePZ_PY_COVER(x, y, 0);
                case 3: return RotateNX_PZ_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNZ_NY_COVER(0, y, 0);
                case 2: return RotateNZ_PY_COVER(0, y, 0);
                case 3: return RotatePZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NY_COVER;
                case 2: return ColPriv.NZ_NY_COVER;
                case 3: return ColPriv.NX_NY_COVER;
            }
            return ColPriv.PZ_NY_COVER;
        }
        private static ColPriv RotatePX_NZ_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNZ_PY_COVER(x, y, 0);
                case 2: return RotateNX_NZ_COVER(x, y, 0);
                case 3: return RotateNZ_NY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PY_COVER(0, y, 0);
                case 2: return RotatePX_PZ_COVER(0, y, 0);
                case 3: return RotatePX_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_COVER;
                case 2: return ColPriv.NX_PZ_COVER;
                case 3: return ColPriv.PX_PZ_COVER;
            }
            return ColPriv.PX_NZ_COVER;
        }
        private static ColPriv RotateNX_NZ_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNZ_NY_COVER(x, y, 0);
                case 2: return RotatePX_NZ_COVER(x, y, 0);
                case 3: return RotateNZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PY_COVER(0, y, 0);
                case 2: return RotateNX_PZ_COVER(0, y, 0);
                case 3: return RotateNX_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_COVER;
                case 2: return ColPriv.PX_PZ_COVER;
                case 3: return ColPriv.PX_NZ_COVER;
            }
            return ColPriv.NX_NZ_COVER;
        }
        private static ColPriv RotateNX_PZ_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePZ_NY_COVER(x, y, 0);
                case 2: return RotatePX_PZ_COVER(x, y, 0);
                case 3: return RotatePZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NY_COVER(0, y, 0);
                case 2: return RotateNX_NZ_COVER(0, y, 0);
                case 3: return RotateNX_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_COVER;
                case 2: return ColPriv.PX_NZ_COVER;
                case 3: return ColPriv.NX_NZ_COVER;
            }
            return ColPriv.NX_PZ_COVER;
        }
        private static ColPriv RotatePX_PZ_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePZ_PY_COVER(x, y, 0);
                case 2: return RotateNX_PZ_COVER(x, y, 0);
                case 3: return RotatePZ_NY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NY_COVER(0, y, 0);
                case 2: return RotatePX_NZ_COVER(0, y, 0);
                case 3: return RotatePX_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_COVER;
                case 2: return ColPriv.NX_NZ_COVER;
                case 3: return ColPriv.NX_PZ_COVER;
            }
            return ColPriv.PX_PZ_COVER;
        }
        private static ColPriv RotatePX_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PY_COVER(x, y, 0);
                case 2: return RotateNX_NY_COVER(x, y, 0);
                case 3: return RotatePX_NY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_COVER(0, y, 0);
                case 2: return RotatePX_NY_COVER(0, y, 0);
                case 3: return RotatePX_NZ_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NZ_PY_COVER;
                case 2: return ColPriv.NX_PY_COVER;
                case 3: return ColPriv.PZ_PY_COVER;
            }
            return ColPriv.PX_PY_COVER;
        }
        private static ColPriv RotateNZ_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_COVER(x, y, 0);
                case 2: return RotateNZ_NY_COVER(x, y, 0);
                case 3: return RotatePX_NZ_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePZ_PY_COVER(0, y, 0);
                case 2: return RotatePZ_NY_COVER(0, y, 0);
                case 3: return RotateNZ_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PY_COVER;
                case 2: return ColPriv.PZ_PY_COVER;
                case 3: return ColPriv.PX_PY_COVER;
            }
            return ColPriv.NZ_PY_COVER;
        }
        private static ColPriv RotateNX_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NY_COVER(x, y, 0);
                case 2: return RotatePX_NY_COVER(x, y, 0);
                case 3: return RotatePX_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_COVER(0, y, 0);
                case 2: return RotateNX_NY_COVER(0, y, 0);
                case 3: return RotateNX_NZ_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PZ_PY_COVER;
                case 2: return ColPriv.PX_PY_COVER;
                case 3: return ColPriv.NZ_PY_COVER;
            }
            return ColPriv.NX_PY_COVER;
        }
        private static ColPriv RotatePZ_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_COVER(x, y, 0);
                case 2: return RotatePZ_NY_COVER(x, y, 0);
                case 3: return RotatePX_PZ_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePZ_NY_COVER(0, y, 0);
                case 2: return RotateNZ_NY_COVER(0, y, 0);
                case 3: return RotateNZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PY_COVER;
                case 2: return ColPriv.NZ_PY_COVER;
                case 3: return ColPriv.NX_PY_COVER;
            }
            return ColPriv.PZ_PY_COVER;
        }
        private static ColPriv RotatePX_PZ_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_NY_COVER(x, y, 0);
                case 2: return RotateNX_PZ_PY_COVER(x, y, 0);
                case 3: return RotatePX_PZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_NY_COVER(0, y, 0);
                case 2: return RotatePX_NZ_PY_COVER(0, y, 0);
                case 3: return RotatePX_PZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_NY_COVER;
                case 2: return ColPriv.NX_NZ_NY_COVER;
                case 3: return ColPriv.NX_PZ_NY_COVER;
            }
            return ColPriv.PX_PZ_NY_COVER;
        }
        private static ColPriv RotatePX_NZ_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_NY_COVER(x, y, 0);
                case 2: return RotateNX_NZ_PY_COVER(x, y, 0);
                case 3: return RotatePX_NZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_NY_COVER(0, y, 0);
                case 2: return RotatePX_PZ_PY_COVER(0, y, 0);
                case 3: return RotatePX_NZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_NY_COVER;
                case 2: return ColPriv.NX_PZ_NY_COVER;
                case 3: return ColPriv.PX_PZ_NY_COVER;
            }
            return ColPriv.PX_NZ_NY_COVER;
        }
        private static ColPriv RotateNX_NZ_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_NY_COVER(x, y, 0);
                case 2: return RotatePX_NZ_PY_COVER(x, y, 0);
                case 3: return RotateNX_NZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_PY_COVER(0, y, 0);
                case 2: return RotateNX_PZ_PY_COVER(0, y, 0);
                case 3: return RotateNX_PZ_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_NY_COVER;
                case 2: return ColPriv.PX_PZ_NY_COVER;
                case 3: return ColPriv.PX_NZ_NY_COVER;
            }
            return ColPriv.NX_NZ_NY_COVER;
        }
        private static ColPriv RotateNX_PZ_NY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_NY_COVER(x, y, 0);
                case 2: return RotatePX_PZ_PY_COVER(x, y, 0);
                case 3: return RotateNX_PZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_NY_COVER(0, y, 0);
                case 2: return RotateNX_NZ_PY_COVER(0, y, 0);
                case 3: return RotateNX_PZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_NY_COVER;
                case 2: return ColPriv.PX_NZ_NY_COVER;
                case 3: return ColPriv.NX_NZ_NY_COVER;
            }
            return ColPriv.NX_PZ_NY_COVER;
        }
        private static ColPriv RotatePX_PZ_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_PY_COVER(x, y, 0);
                case 2: return RotateNX_PZ_NY_COVER(x, y, 0);
                case 3: return RotatePX_PZ_NY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_NY_COVER(0, y, 0);
                case 2: return RotatePX_NZ_NY_COVER(0, y, 0);
                case 3: return RotatePX_NZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_PY_COVER;
                case 2: return ColPriv.NX_NZ_PY_COVER;
                case 3: return ColPriv.NX_PZ_PY_COVER;
            }
            return ColPriv.PX_PZ_PY_COVER;
        }
        private static ColPriv RotatePX_NZ_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_PY_COVER(x, y, 0);
                case 2: return RotateNX_NZ_NY_COVER(x, y, 0);
                case 3: return RotatePX_NZ_NY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_PY_COVER(0, y, 0);
                case 2: return RotatePX_PZ_NY_COVER(0, y, 0);
                case 3: return RotatePX_NZ_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_PY_COVER;
                case 2: return ColPriv.NX_PZ_PY_COVER;
                case 3: return ColPriv.PX_PZ_PY_COVER;
            }
            return ColPriv.PX_NZ_PY_COVER;
        }
        private static ColPriv RotateNX_NZ_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_NY_COVER(x, y, 0);
                case 2: return RotatePX_NZ_NY_COVER(x, y, 0);
                case 3: return RotatePX_NZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_PY_COVER(0, y, 0);
                case 2: return RotateNX_PZ_NY_COVER(0, y, 0);
                case 3: return RotateNX_NZ_NY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_PY_COVER;
                case 2: return ColPriv.PX_PZ_PY_COVER;
                case 3: return ColPriv.PX_NZ_PY_COVER;
            }
            return ColPriv.NX_NZ_PY_COVER;
        }
        private static ColPriv RotateNX_PZ_PY_COVER(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_NY_COVER(x, y, 0);
                case 2: return RotatePX_PZ_NY_COVER(x, y, 0);
                case 3: return RotatePX_PZ_PY_COVER(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_NY_COVER(0, y, 0);
                case 2: return RotateNX_NZ_NY_COVER(0, y, 0);
                case 3: return RotateNX_NZ_PY_COVER(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_PY_COVER;
                case 2: return ColPriv.PX_NZ_PY_COVER;
                case 3: return ColPriv.NX_NZ_PY_COVER;
            }
            return ColPriv.NX_PZ_PY_COVER;
        }
        private static ColPriv RotatePX_PZ_NY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_PY_INCOV(x, y, 0);
                case 2: return RotateNX_PZ_PY_INCOV(x, y, 0);
                case 3: return RotateNX_PZ_NY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_NY_INCOV(0, y, 0);
                case 2: return RotatePX_NZ_PY_INCOV(0, y, 0);
                case 3: return RotatePX_PZ_PY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_NY_INCOV;
                case 2: return ColPriv.NX_NZ_NY_INCOV;
                case 3: return ColPriv.NX_PZ_NY_INCOV;
            }
            return ColPriv.PX_PZ_NY_INCOV;
        }
        private static ColPriv RotatePX_NZ_NY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_PY_INCOV(x, y, 0);
                case 2: return RotateNX_NZ_PY_INCOV(x, y, 0);
                case 3: return RotateNX_NZ_NY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NZ_PY_INCOV(0, y, 0);
                case 2: return RotatePX_PZ_PY_INCOV(0, y, 0);
                case 3: return RotatePX_PZ_NY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_NY_INCOV;
                case 2: return ColPriv.NX_PZ_NY_INCOV;
                case 3: return ColPriv.PX_PZ_NY_INCOV;
            }
            return ColPriv.PX_NZ_NY_INCOV;
        }
        private static ColPriv RotateNX_NZ_NY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NZ_NY_INCOV(x, y, 0);
                case 2: return RotatePX_NZ_PY_INCOV(x, y, 0);
                case 3: return RotateNX_NZ_PY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_PY_INCOV(0, y, 0);
                case 2: return RotateNX_PZ_PY_INCOV(0, y, 0);
                case 3: return RotateNX_PZ_NY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_NY_INCOV;
                case 2: return ColPriv.PX_PZ_NY_INCOV;
                case 3: return ColPriv.PX_NZ_NY_INCOV;
            }
            return ColPriv.NX_NZ_NY_INCOV;
        }
        private static ColPriv RotateNX_PZ_NY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_PZ_NY_INCOV(x, y, 0);
                case 2: return RotatePX_PZ_PY_INCOV(x, y, 0);
                case 3: return RotateNX_PZ_PY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NZ_NY_INCOV(0, y, 0);
                case 2: return RotateNX_NZ_PY_INCOV(0, y, 0);
                case 3: return RotateNX_PZ_PY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_NY_INCOV;
                case 2: return ColPriv.PX_NZ_NY_INCOV;
                case 3: return ColPriv.NX_NZ_NY_INCOV;
            }
            return ColPriv.NX_PZ_NY_INCOV;
        }
        private static ColPriv RotatePX_PZ_PY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_PY_INCOV(x, y, 0);
                case 2: return RotateNX_PZ_NY_INCOV(x, y, 0);
                case 3: return RotatePX_PZ_NY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_NY_INCOV(0, y, 0);
                case 2: return RotatePX_NZ_NY_INCOV(0, y, 0);
                case 3: return RotatePX_NZ_PY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NZ_PY_INCOV;
                case 2: return ColPriv.NX_NZ_PY_INCOV;
                case 3: return ColPriv.NX_PZ_PY_INCOV;
            }
            return ColPriv.PX_PZ_PY_INCOV;
        }
        private static ColPriv RotateNX_PZ_PY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_PZ_NY_INCOV(x, y, 0);
                case 2: return RotatePX_PZ_NY_INCOV(x, y, 0);
                case 3: return RotatePX_PZ_PY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_NY_INCOV(0, y, 0);
                case 2: return RotateNX_NZ_NY_INCOV(0, y, 0);
                case 3: return RotateNX_NZ_PY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_PZ_PY_INCOV;
                case 2: return ColPriv.PX_NZ_PY_INCOV;
                case 3: return ColPriv.NX_NZ_PY_INCOV;
            }
            return ColPriv.NX_PZ_PY_INCOV;
        }
        private static ColPriv RotateNX_NZ_PY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_NY_INCOV(x, y, 0);
                case 2: return RotatePX_NZ_NY_INCOV(x, y, 0);
                case 3: return RotatePX_NZ_PY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_PZ_PY_INCOV(0, y, 0);
                case 2: return RotateNX_PZ_NY_INCOV(0, y, 0);
                case 3: return RotateNX_NZ_NY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_PZ_PY_INCOV;
                case 2: return ColPriv.PX_PZ_PY_INCOV;
                case 3: return ColPriv.PX_NZ_PY_INCOV;
            }
            return ColPriv.NX_NZ_PY_INCOV;
        }
        private static ColPriv RotatePX_NZ_PY_INCOV(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NZ_PY_INCOV(x, y, 0);
                case 2: return RotateNX_NZ_NY_INCOV(x, y, 0);
                case 3: return RotatePX_NZ_NY_INCOV(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_PZ_PY_INCOV(0, y, 0);
                case 2: return RotatePX_PZ_NY_INCOV(0, y, 0);
                case 3: return RotatePX_NZ_NY_INCOV(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NZ_PY_INCOV;
                case 2: return ColPriv.NX_PZ_PY_INCOV;
                case 3: return ColPriv.PX_PZ_PY_INCOV;
            }
            return ColPriv.PX_NZ_PY_INCOV;
        }
        private static ColPriv RotateX_PILLAR(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateY_PILLAR(x, y, 0);
                case 2: return RotateX_PILLAR(x, y, 0);
                case 3: return RotateY_PILLAR(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateX_PILLAR(0, y, 0);
                case 2: return RotateX_PILLAR(0, y, 0);
                case 3: return RotateX_PILLAR(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Z_PILLAR;
                case 2: return ColPriv.X_PILLAR;
                case 3: return ColPriv.Z_PILLAR;
            }
            return ColPriv.X_PILLAR;
        }
        private static ColPriv RotateZ_PILLAR(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateZ_PILLAR(x, y, 0);
                case 2: return RotateZ_PILLAR(x, y, 0);
                case 3: return RotateZ_PILLAR(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateY_PILLAR(0, y, 0);
                case 2: return RotateZ_PILLAR(0, y, 0);
                case 3: return RotateY_PILLAR(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.X_PILLAR;
                case 2: return ColPriv.Z_PILLAR;
                case 3: return ColPriv.X_PILLAR;
            }
            return ColPriv.Z_PILLAR;
        }
        private static ColPriv RotateY_PILLAR(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateX_PILLAR(x, y, 0);
                case 2: return RotateY_PILLAR(x, y, 0);
                case 3: return RotateX_PILLAR(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateZ_PILLAR(0, y, 0);
                case 2: return RotateY_PILLAR(0, y, 0);
                case 3: return RotateZ_PILLAR(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Y_PILLAR;
                case 2: return ColPriv.Y_PILLAR;
                case 3: return ColPriv.Y_PILLAR;
            }
            return ColPriv.Y_PILLAR;
        }
        private static ColPriv RotateNX_NUB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNY_NUB(x, y, 0);
                case 2: return RotatePX_NUB(x, y, 0);
                case 3: return RotatePY_NUB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNX_NUB(0, y, 0);
                case 2: return RotateNX_NUB(0, y, 0);
                case 3: return RotateNX_NUB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PZ_NUB;
                case 2: return ColPriv.PX_NUB;
                case 3: return ColPriv.NZ_NUB;
            }
            return ColPriv.NX_NUB;
        }
        private static ColPriv RotatePX_NUB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePY_NUB(x, y, 0);
                case 2: return RotateNX_NUB(x, y, 0);
                case 3: return RotateNY_NUB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePX_NUB(0, y, 0);
                case 2: return RotatePX_NUB(0, y, 0);
                case 3: return RotatePX_NUB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NZ_NUB;
                case 2: return ColPriv.NX_NUB;
                case 3: return ColPriv.PZ_NUB;
            }
            return ColPriv.PX_NUB;
        }
        private static ColPriv RotateNZ_NUB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNZ_NUB(x, y, 0);
                case 2: return RotateNZ_NUB(x, y, 0);
                case 3: return RotateNZ_NUB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePY_NUB(0, y, 0);
                case 2: return RotatePZ_NUB(0, y, 0);
                case 3: return RotateNY_NUB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NX_NUB;
                case 2: return ColPriv.PZ_NUB;
                case 3: return ColPriv.PX_NUB;
            }
            return ColPriv.NZ_NUB;
        }
        private static ColPriv RotatePZ_NUB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePZ_NUB(x, y, 0);
                case 2: return RotatePZ_NUB(x, y, 0);
                case 3: return RotatePZ_NUB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNY_NUB(0, y, 0);
                case 2: return RotateNZ_NUB(0, y, 0);
                case 3: return RotatePY_NUB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PX_NUB;
                case 2: return ColPriv.NZ_NUB;
                case 3: return ColPriv.NX_NUB;
            }
            return ColPriv.PZ_NUB;
        }
        private static ColPriv RotatePY_NUB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateNX_NUB(x, y, 0);
                case 2: return RotateNY_NUB(x, y, 0);
                case 3: return RotatePX_NUB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotatePZ_NUB(0, y, 0);
                case 2: return RotateNY_NUB(0, y, 0);
                case 3: return RotateNZ_NUB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.PY_NUB;
                case 2: return ColPriv.PY_NUB;
                case 3: return ColPriv.PY_NUB;
            }
            return ColPriv.PY_NUB;
        }
        private static ColPriv RotateNY_NUB(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotatePX_NUB(x, y, 0);
                case 2: return RotatePY_NUB(x, y, 0);
                case 3: return RotateNX_NUB(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateNZ_NUB(0, y, 0);
                case 2: return RotatePY_NUB(0, y, 0);
                case 3: return RotatePZ_NUB(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.NY_NUB;
                case 2: return ColPriv.NY_NUB;
                case 3: return ColPriv.NY_NUB;
            }
            return ColPriv.NY_NUB;
        }
        private static ColPriv RotateX_RIDGE_NY(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateY_RIDGE_PX(x, y, 0);
                case 2: return RotateX_RIDGE_PY(x, y, 0);
                case 3: return RotateY_RIDGE_NX(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateX_RIDGE_NZ(0, y, 0);
                case 2: return RotateX_RIDGE_PY(0, y, 0);
                case 3: return RotateX_RIDGE_PZ(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Z_RIDGE_NY;
                case 2: return ColPriv.X_RIDGE_NY;
                case 3: return ColPriv.Z_RIDGE_NY;
            }
            return ColPriv.X_RIDGE_NY;
        }
        private static ColPriv RotateZ_RIDGE_NY(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateZ_RIDGE_PX(x, y, 0);
                case 2: return RotateZ_RIDGE_PY(x, y, 0);
                case 3: return RotateZ_RIDGE_NX(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateY_RIDGE_NZ(0, y, 0);
                case 2: return RotateZ_RIDGE_PY(0, y, 0);
                case 3: return RotateY_RIDGE_PZ(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.X_RIDGE_NY;
                case 2: return ColPriv.Z_RIDGE_NY;
                case 3: return ColPriv.X_RIDGE_NY;
            }
            return ColPriv.Z_RIDGE_NY;
        }
        private static ColPriv RotateX_RIDGE_PY(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateY_RIDGE_NX(x, y, 0);
                case 2: return RotateX_RIDGE_NY(x, y, 0);
                case 3: return RotateY_RIDGE_PX(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateX_RIDGE_PZ(0, y, 0);
                case 2: return RotateX_RIDGE_NY(0, y, 0);
                case 3: return RotateX_RIDGE_NZ(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Z_RIDGE_PY;
                case 2: return ColPriv.X_RIDGE_PY;
                case 3: return ColPriv.Z_RIDGE_PY;
            }
            return ColPriv.X_RIDGE_PY;
        }
        private static ColPriv RotateZ_RIDGE_PY(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateZ_RIDGE_NX(x, y, 0);
                case 2: return RotateZ_RIDGE_NY(x, y, 0);
                case 3: return RotateZ_RIDGE_PX(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateY_RIDGE_PZ(0, y, 0);
                case 2: return RotateZ_RIDGE_NY(0, y, 0);
                case 3: return RotateY_RIDGE_NZ(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.X_RIDGE_PY;
                case 2: return ColPriv.Z_RIDGE_PY;
                case 3: return ColPriv.X_RIDGE_PY;
            }
            return ColPriv.Z_RIDGE_PY;
        }
        private static ColPriv RotateY_RIDGE_PX(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateX_RIDGE_PY(x, y, 0);
                case 2: return RotateY_RIDGE_NX(x, y, 0);
                case 3: return RotateX_RIDGE_NY(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateZ_RIDGE_PX(0, y, 0);
                case 2: return RotateY_RIDGE_PX(0, y, 0);
                case 3: return RotateZ_RIDGE_PX(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Y_RIDGE_NZ;
                case 2: return ColPriv.Y_RIDGE_NX;
                case 3: return ColPriv.Y_RIDGE_PZ;
            }
            return ColPriv.Y_RIDGE_PX;
        }
        private static ColPriv RotateZ_RIDGE_PX(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateZ_RIDGE_PY(x, y, 0);
                case 2: return RotateZ_RIDGE_NX(x, y, 0);
                case 3: return RotateZ_RIDGE_NY(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateY_RIDGE_PX(0, y, 0);
                case 2: return RotateZ_RIDGE_PX(0, y, 0);
                case 3: return RotateY_RIDGE_PX(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.X_RIDGE_NZ;
                case 2: return ColPriv.Z_RIDGE_NX;
                case 3: return ColPriv.X_RIDGE_PZ;
            }
            return ColPriv.Z_RIDGE_PX;
        }
        private static ColPriv RotateY_RIDGE_NX(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateX_RIDGE_NY(x, y, 0);
                case 2: return RotateY_RIDGE_PX(x, y, 0);
                case 3: return RotateX_RIDGE_PY(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateZ_RIDGE_NX(0, y, 0);
                case 2: return RotateY_RIDGE_NX(0, y, 0);
                case 3: return RotateZ_RIDGE_NX(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Y_RIDGE_PZ;
                case 2: return ColPriv.Y_RIDGE_PX;
                case 3: return ColPriv.Y_RIDGE_NZ;
            }
            return ColPriv.Y_RIDGE_NX;
        }
        private static ColPriv RotateZ_RIDGE_NX(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateZ_RIDGE_NY(x, y, 0);
                case 2: return RotateZ_RIDGE_PX(x, y, 0);
                case 3: return RotateZ_RIDGE_PY(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateY_RIDGE_NX(0, y, 0);
                case 2: return RotateZ_RIDGE_NX(0, y, 0);
                case 3: return RotateY_RIDGE_NX(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.X_RIDGE_PZ;
                case 2: return ColPriv.Z_RIDGE_PX;
                case 3: return ColPriv.X_RIDGE_NZ;
            }
            return ColPriv.Z_RIDGE_NX;
        }
        private static ColPriv RotateY_RIDGE_NZ(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateX_RIDGE_NZ(x, y, 0);
                case 2: return RotateY_RIDGE_NZ(x, y, 0);
                case 3: return RotateX_RIDGE_NZ(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateZ_RIDGE_PY(0, y, 0);
                case 2: return RotateY_RIDGE_PZ(0, y, 0);
                case 3: return RotateZ_RIDGE_NY(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Y_RIDGE_NX;
                case 2: return ColPriv.Y_RIDGE_PZ;
                case 3: return ColPriv.Y_RIDGE_PX;
            }
            return ColPriv.Y_RIDGE_NZ;
        }
        private static ColPriv RotateX_RIDGE_NZ(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateY_RIDGE_NZ(x, y, 0);
                case 2: return RotateX_RIDGE_NZ(x, y, 0);
                case 3: return RotateY_RIDGE_NZ(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateX_RIDGE_PY(0, y, 0);
                case 2: return RotateX_RIDGE_PZ(0, y, 0);
                case 3: return RotateX_RIDGE_NY(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Z_RIDGE_NX;
                case 2: return ColPriv.X_RIDGE_PZ;
                case 3: return ColPriv.Z_RIDGE_PX;
            }
            return ColPriv.X_RIDGE_NZ;
        }
        private static ColPriv RotateY_RIDGE_PZ(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateX_RIDGE_PZ(x, y, 0);
                case 2: return RotateY_RIDGE_PZ(x, y, 0);
                case 3: return RotateX_RIDGE_PZ(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateZ_RIDGE_NY(0, y, 0);
                case 2: return RotateY_RIDGE_NZ(0, y, 0);
                case 3: return RotateZ_RIDGE_PY(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Y_RIDGE_PX;
                case 2: return ColPriv.Y_RIDGE_NZ;
                case 3: return ColPriv.Y_RIDGE_NX;
            }
            return ColPriv.Y_RIDGE_PZ;
        }
        private static ColPriv RotateX_RIDGE_PZ(int x, int y, int z)
        {
            switch (z)
            {
                case 1: return RotateY_RIDGE_PZ(x, y, 0);
                case 2: return RotateX_RIDGE_PZ(x, y, 0);
                case 3: return RotateY_RIDGE_PZ(x, y, 0);
            }
            switch (x)
            {
                case 1: return RotateX_RIDGE_NY(0, y, 0);
                case 2: return RotateX_RIDGE_NZ(0, y, 0);
                case 3: return RotateX_RIDGE_PY(0, y, 0);
            }
            switch (y)
            {
                case 1: return ColPriv.Z_RIDGE_PX;
                case 2: return ColPriv.X_RIDGE_NZ;
                case 3: return ColPriv.Z_RIDGE_NX;
            }
            return ColPriv.X_RIDGE_PZ;
        }

        public delegate int EdgeRotate(int rotation);

        // XXXXXXXX YYYYYYYY ZZZZZZZZ WWWWWWWW
        //                            WWWWWWWW -- Collider primitive shape
        //                   ZZZZZZZZ -- Alteration shape data
        //     PXNX PYNYPZNZ          -- Flat/Convex/Concave OVERRIDES for sides
        private static ColPrivAlterations rotateoverridemetadata(ColPrivAlterations shape, int x, int y, int z)
        {
            int extra = (int)shape & (int)ColPrivAlterations.ALTERATIONS_ONLY;
            int nshp = (int)shape;
            int px = nshp & (3 << 26);
            int nx = nshp & (3 << 24);
            int py = nshp & (3 << 22);
            int ny = nshp & (3 << 20);
            int pz = nshp & (3 << 18);
            int nz = nshp & (3 << 16);

            int replaced = pz | nz;
            switch (z)
            {
                case 0:
                    replaced |= px;
                    replaced |= nx;
                    replaced |= py;
                    replaced |= ny;
                    break;
                case 1: // py -> nx, nx -> ny, ny -> px, px -> py
                    replaced |= (py << 2);
                    replaced |= (nx >> 4);
                    replaced |= (ny << 6);
                    replaced |= (px >> 4);
                    break;
                case 2: // py -> ny, ny -> py, nx -> px, px -> nx
                    replaced |= (py >> 2);
                    replaced |= (nx << 2);
                    replaced |= (ny << 2);
                    replaced |= (px >> 2);
                    break;
                case 3: // py <- nx, nx <- ny, ny <- px, px <- py
                    replaced |= (nx >> 2);
                    replaced |= (ny << 4);
                    replaced |= (px >> 6);
                    replaced |= (py << 4);
                    break;
            }

            px = replaced & (3 << 26);
            nx = replaced & (3 << 24);
            py = replaced & (3 << 22);
            ny = replaced & (3 << 20);
            pz = replaced & (3 << 18);
            nz = replaced & (3 << 16);
            replaced = px | nx;

            switch (x)
            {
                case 0:
                    replaced |= pz;
                    replaced |= nz;
                    replaced |= pz;
                    replaced |= nz;
                    break;
                case 1: // py->pz, pz->ny, ny->nz, nz->py
                    replaced |= (py >> 4);
                    replaced |= (pz << 2);
                    replaced |= (ny >> 4);
                    replaced |= (nz << 6);
                    break;
                case 2:
                    replaced |= (py >> 2);
                    replaced |= (ny << 2);
                    replaced |= (pz >> 2);
                    replaced |= (nz << 2);
                    break;
                case 3: // pz->py, ny->pz, nz->ny, py->nz
                    replaced |= (pz << 4);
                    replaced |= (ny >> 2);
                    replaced |= (nz << 4);
                    replaced |= (py >> 6);
                    break;
            }

            px = replaced & (3 << 26);
            nx = replaced & (3 << 24);
            py = replaced & (3 << 22);
            ny = replaced & (3 << 20);
            pz = replaced & (3 << 18);
            nz = replaced & (3 << 16);
            replaced = py | ny;

            switch (y)
            {
                case 0:
                    replaced |= px;
                    replaced |= nx;
                    replaced |= pz;
                    replaced |= nz;
                    break;
                case 1: //px _> nz, nz -> nx, nx -> pz, pz -> px
                    replaced |= (px >> 10);
                    replaced |= (nz << 8);
                    replaced |= (nx >> 6);
                    replaced |= (pz << 8);
                    break;
                case 2:
                    replaced |= (px >> 2);
                    replaced |= (nx << 2);
                    replaced |= (pz >> 2);
                    replaced |= (nz << 2);
                    break;
                case 3: // nz > px, nx > nz, pz > nx, px > pz
                    replaced |= (nz << 10);
                    replaced |= (nx >> 8);
                    replaced |= (pz << 6);
                    replaced |= (px >> 8);
                    break;
            }

            return (ColPrivAlterations)(extra | replaced);
        }


        //   |\                  |\
        //   |  \                |  \
        // S |    \            S |    \
        // I |     \           I |     \
        // D | +XYZ \          D | -XYZ \
        // E |        \        E |        \
        // 2 |__________\      1 |__________\
        //     S I D E 1           S I D E 2
        // Referenced side for 1x2 blocks is the side that's NOT full.

        private enum SIDES
        {
            PX,
            NX,
            PY,
            NY,
            PZ,
            NZ
        }
        private static bool EdgeCondition(SIDES blockside, int shapetype, float side1, float side2)
        {
            int shapeOverride = 0;
            switch (blockside)
            {
                case SIDES.PX:
                    shapeOverride = (shapetype >> 26) & 3;
                    break;
                case SIDES.PY:
                    shapeOverride = (shapetype >> 22) & 3;
                    break;
                case SIDES.PZ:
                    shapeOverride = (shapetype >> 18) & 3;
                    break;
                case SIDES.NX:
                    shapeOverride = (shapetype >> 24) & 3;
                    break;
                case SIDES.NY:
                    shapeOverride = (shapetype >> 20) & 3;
                    break;
                case SIDES.NZ:
                    shapeOverride = (shapetype >> 16) & 3;
                    break;
            }
            var shape = shapetype;


            if (shape == (int)ColPrivAlterations.FLAT || shapeOverride == 1)
                return side1 + side2 <= 1;
            else if (shape == (int)ColPrivAlterations.CONVEX || shapeOverride == 2)
                return side1 * side1 + side2 * side2 <= 1;
            else if (shape == (int)ColPrivAlterations.CONCAVE || shapeOverride == 3)
            {
                float m1 = 1 - side1;
                float m2 = 1 - side2;
                return m1 * m1 + m2 * m2 <= 1;
            }

            int side = ((shape & (int)ColPrivAlterations.PXSIDE1) != 0 || (shape & (int)ColPrivAlterations.PYSIDE1) != 0
                || (shape & (int)ColPrivAlterations.PZSIDE1) != 0) ? 1 : 2;

            bool concave = (shape & (int)ColPrivAlterations.ALTERATIONS_ONLY) == (int)ColPrivAlterations.CONCAVE;
            bool convex = (shape & (int)ColPrivAlterations.ALTERATIONS_ONLY) == (int)ColPrivAlterations.CONVEX;
            bool high = (shape & (int)ColPrivAlterations.ALTERATIONS_ONLY) == (int)ColPrivAlterations.HIGH;

            if ((shape & (int)ColPrivAlterations.ALTERATIONS_ONLY) == (int)ColPrivAlterations.FULLSIDE)
                return true;

            if (!high)
            { // LOW
                if (concave)
                {
                    //       return (side == 2) ? () : ();
                }
                else if (convex)
                {
                    //        return side == 2 ? () : ();
                }
                else
                {
                    return (side == 2) ? (side1 + 2 * side2 <= 1) : (2 * side1 + side2 <= 1);
                }
            }
            else
            { // HIGH
                if (concave)
                {

                }
                else if (convex)
                {

                }
                else
                {
                    return (side == 2) ? (side1 + 2 * (side2 - .5f) <= 1) : (2 * (side1 - .5f) + side2 <= 1);
                }
            }


            return false;

        }

        public delegate bool FaceCast(int shape, Ray ray, int position, ref double dist);
        private static void GetPXFaceData(Ray ray, ref int x, ref int y, ref int z, ref float t, ref float rz, ref float ry, ref double dist, int position)
        {
            x = BlockData.GetX(position);
            y = BlockData.GetY(position);
            z = BlockData.GetZ(position);
            float facePosition = x + 0.5f;
            t = (facePosition - ray.origin.x) / ray.direction.x;
            rz = ray.origin.z + ray.direction.z * t;
            ry = ray.origin.y + ray.direction.y * t;
            dist = ray.direction.magnitude * t;
        }
        private static void GetPYFaceData(Ray ray, ref int x, ref int y, ref int z, ref float t, ref float rz, ref float rx, ref double dist, int position)
        {
            x = BlockData.GetX(position);
            y = BlockData.GetY(position);
            z = BlockData.GetZ(position);
            float facePosition = y + 0.5f;
            t = (facePosition - ray.origin.y) / ray.direction.y;
            rz = ray.origin.z + ray.direction.z * t;
            rx = ray.origin.x + ray.direction.x * t;
            dist = ray.direction.magnitude * t;
        }
        private static void GetPZFaceData(Ray r, ref int x, ref int y, ref int z, ref float t, ref float rx, ref float ry, ref double dist, int position)
        {
            x = BlockData.GetX(position);
            y = BlockData.GetY(position);
            z = BlockData.GetZ(position);
            float facePosition = z + 0.5f;
            t = (facePosition - r.origin.z) / r.direction.z;
            ry = r.origin.y + r.direction.y * t;
            rx = r.origin.x + r.direction.x * t;
            dist = r.direction.magnitude * t;
        }
        private static void GetNXFaceData(Ray ray, ref int x, ref int y, ref int z, ref float t, ref float rz, ref float ry, ref double dist, int position)
        {
            x = BlockData.GetX(position);
            y = BlockData.GetY(position);
            z = BlockData.GetZ(position);
            float facePosition = x - 0.5f;
            t = (facePosition - ray.origin.x) / ray.direction.x;
            rz = ray.origin.z + ray.direction.z * t;
            ry = ray.origin.y + ray.direction.y * t;
            dist = ray.direction.magnitude * t;
        }
        private static void GetNYFaceData(Ray ray, ref int x, ref int y, ref int z, ref float t, ref float rz, ref float rx, ref double dist, int position)
        {
            x = BlockData.GetX(position);
            y = BlockData.GetY(position);
            z = BlockData.GetZ(position);
            float facePosition = y - 0.5f;
            t = (facePosition - ray.origin.y) / ray.direction.y;
            rz = ray.origin.z + ray.direction.z * t;
            rx = ray.origin.x + ray.direction.x * t;
            dist = ray.direction.magnitude * t;
        }
        private static void GetNZFaceData(Ray r, ref int x, ref int y, ref int z, ref float t, ref float rx, ref float ry, ref double dist, int position)
        {
            x = BlockData.GetX(position);
            y = BlockData.GetY(position);
            z = BlockData.GetZ(position);
            float facePosition = z - 0.5f;
            t = (facePosition - r.origin.z) / r.direction.z;
            ry = r.origin.y + r.direction.y * t;
            rx = r.origin.x + r.direction.x * t;
            dist = r.direction.magnitude * t;
        }
        private static bool CastFacePX_SQUARE(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetPXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f;
        }
        private static bool CastFacePY_SQUARE(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetPYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f;
        }
        private static bool CastFacePZ_SQUARE(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetPZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f;
        }
        private static bool CastFaceNX_SQUARE(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetNXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f;
        }
        private static bool CastFaceNY_SQUARE(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetNYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f;
        }
        private static bool CastFaceNZ_SQUARE(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetNZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f;
        }
        private static bool CastFacePX_TRI_PYNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetPXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.PX, shape, y - ry + .5f, rz - z + .5f);
        }
        private static bool CastFacePX_TRI_NYPZ(int shape, Ray ray, int position, ref double dist)
        {

            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetPXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.PX, shape, ry - y + .5f, z - rz + .5f);// (rz - z) >= (ry - y);
        }
        private static bool CastFacePX_TRI_PYPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetPXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.PX, shape, z - rz + .5f, y - ry + .5f);// -(rz - z) <= (ry - y);
        }
        private static bool CastFacePX_TRI_NYNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetPXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.PX, shape, rz - z + .5f, ry - y + .5f);
        }
        private static bool CastFaceNX_TRI_PYNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetNXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.NX, shape, y - ry + .5f, rz - z + .5f);// (rz - z) <= (ry - y);
        }
        private static bool CastFaceNX_TRI_NYPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetNXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.NX, shape, ry - y + .5f, z - rz + .5f);// (rz - z) >= (ry - y);
        }
        private static bool CastFaceNX_TRI_PYPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetNXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.NX, shape, z - rz + .5f, y - ry + .5f);// -(rz - z) <= (ry - y);
        }
        private static bool CastFaceNX_TRI_NYNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, ry = 0;
            GetNXFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref ry, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && ry >= y - 0.5f && ry < y + 0.5f
                && EdgeCondition(SIDES.NX, shape, rz - z + .5f, ry - y + .5f);// -(rz - z) >= (ry - y);
        }
        private static bool CastFacePZ_TRI_PXPY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetPZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PZ, shape, y - ry + .5f, x - rx + .5f);// (rx - x) >= -(ry - y);
        }
        private static bool CastFacePZ_TRI_NXNY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetPZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PZ, shape, ry - y + .5f, rx - x + .5f);// (rx - x) <= -(ry - y);
        }
        private static bool CastFacePZ_TRI_NXPY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetPZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PZ, shape, rx - x + .5f, y - ry + .5f);// (rx - x) <= (ry - y);
        }
        private static bool CastFacePZ_TRI_PXNY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetPZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PZ, shape, x - rx + .5f, ry - y + .5f);// (rx - x) >= (ry - y);
        }
        private static bool CastFaceNZ_TRI_PXPY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetNZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NZ, shape, y - ry + .5f, x - rx + .5f);// (rx - x) >= -(ry - y);
        }
        private static bool CastFaceNZ_TRI_NXNY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetNZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NZ, shape, ry - y + .5f, rx - x + .5f);//(rx - x) <= -(ry - y);
        }
        private static bool CastFaceNZ_TRI_NXPY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetNZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NZ, shape, rx - x + .5f, y - ry + .5f);// (rx - x) <= (ry - y);
        }
        private static bool CastFaceNZ_TRI_PXNY(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, ry = 0, rx = 0;
            GetNZFaceData(ray, ref x, ref y, ref z, ref t, ref rx, ref ry, ref dist, position);
            return t >= 0 && ry >= y - 0.5f && ry < y + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NZ, shape, x - rx + .5f, ry - y + .5f);// (rx - x) >= (ry - y);
        }
        private static bool CastFacePY_TRI_PXPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetPYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PY, shape, x - rx + .5f, z - rz + .5f);// (rz - z) >= -(rx - x);
        }
        private static bool CastFacePY_TRI_NXNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetPYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PY, shape, rx - x + .5f, rz - z + .5f);// (rz - z) <= -(rx - x);
        }
        private static bool CastFacePY_TRI_PXNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetPYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PY, shape, rz - z + .5f, x - rx + .5f);// (rz - z) <= (rx - x);
        }
        private static bool CastFacePY_TRI_NXPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetPYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.PY, shape, z - rz + .5f, rx - x + .5f);// (rz - z) >= (rx - x);
        }
        private static bool CastFaceNY_TRI_PXPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetNYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NY, shape, x - rx + .5f, z - rz + .5f);// (rz - z) >= -(rx - x);
        }
        private static bool CastFaceNY_TRI_NXNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetNYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NY, shape, rx - x + .5f, rz - z + .5f);// (rz - z) <= -(rx - x);
        }
        private static bool CastFaceNY_TRI_PXNZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetNYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NY, shape, rz - z + .5f, x - rx + .5f);// (rz - z) <= (rx - x);
        }
        private static bool CastFaceNY_TRI_NXPZ(int shape, Ray ray, int position, ref double dist)
        {
            int x = 0, y = 0, z = 0;
            float t = 0, rz = 0, rx = 0;
            GetNYFaceData(ray, ref x, ref y, ref z, ref t, ref rz, ref rx, ref dist, position);
            return t >= 0 && rz >= z - 0.5f && rz < z + 0.5f && rx >= x - 0.5f && rx < x + 0.5f &&
                EdgeCondition(SIDES.NY, shape, z - rz + .5f, rx - x + .5f);// (rz - z) >= (rx - x);
        }
        private static bool CastFace_False(int shape, Ray ray, int position, ref double dist)
        {
            return false;
        }
    }
}