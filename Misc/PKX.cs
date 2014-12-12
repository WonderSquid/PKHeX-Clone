﻿using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace PKHeX
{
    public partial class PKX
    {
        // C# PKX Function Library
        // No WinForm object related code, only to calculate information.
        // Relies on Util for some common operations.

        // Data
        public static uint LCRNG(uint seed)
        {
            uint a = 0x41C64E6D;
            uint c = 0x00006073;

            seed = (seed * a + c) & 0xFFFFFFFF;
            return seed;
        }

        public static PersonalParser PersonalGetter = new PersonalParser();
       
        public static DataTable MovePPTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Move", typeof(int));
            table.Columns.Add("PP", typeof(int));
            table.Rows.Add(0, 0);
            table.Rows.Add(1, 35);
            table.Rows.Add(2, 25);
            table.Rows.Add(3, 10);
            table.Rows.Add(4, 15);
            table.Rows.Add(5, 20);
            table.Rows.Add(6, 20);
            table.Rows.Add(7, 15);
            table.Rows.Add(8, 15);
            table.Rows.Add(9, 15);
            table.Rows.Add(10, 35);
            table.Rows.Add(11, 30);
            table.Rows.Add(12, 5);
            table.Rows.Add(13, 10);
            table.Rows.Add(14, 20);
            table.Rows.Add(15, 30);
            table.Rows.Add(16, 35);
            table.Rows.Add(17, 35);
            table.Rows.Add(18, 20);
            table.Rows.Add(19, 15);
            table.Rows.Add(20, 20);
            table.Rows.Add(21, 20);
            table.Rows.Add(22, 25);
            table.Rows.Add(23, 20);
            table.Rows.Add(24, 30);
            table.Rows.Add(25, 5);
            table.Rows.Add(26, 10);
            table.Rows.Add(27, 15);
            table.Rows.Add(28, 15);
            table.Rows.Add(29, 15);
            table.Rows.Add(30, 25);
            table.Rows.Add(31, 20);
            table.Rows.Add(32, 5);
            table.Rows.Add(33, 35);
            table.Rows.Add(34, 15);
            table.Rows.Add(35, 20);
            table.Rows.Add(36, 20);
            table.Rows.Add(37, 10);
            table.Rows.Add(38, 15);
            table.Rows.Add(39, 30);
            table.Rows.Add(40, 35);
            table.Rows.Add(41, 20);
            table.Rows.Add(42, 20);
            table.Rows.Add(43, 30);
            table.Rows.Add(44, 25);
            table.Rows.Add(45, 40);
            table.Rows.Add(46, 20);
            table.Rows.Add(47, 15);
            table.Rows.Add(48, 20);
            table.Rows.Add(49, 20);
            table.Rows.Add(50, 20);
            table.Rows.Add(51, 30);
            table.Rows.Add(52, 25);
            table.Rows.Add(53, 15);
            table.Rows.Add(54, 30);
            table.Rows.Add(55, 25);
            table.Rows.Add(56, 5);
            table.Rows.Add(57, 15);
            table.Rows.Add(58, 10);
            table.Rows.Add(59, 5);
            table.Rows.Add(60, 20);
            table.Rows.Add(61, 20);
            table.Rows.Add(62, 20);
            table.Rows.Add(63, 5);
            table.Rows.Add(64, 35);
            table.Rows.Add(65, 20);
            table.Rows.Add(66, 25);
            table.Rows.Add(67, 20);
            table.Rows.Add(68, 20);
            table.Rows.Add(69, 20);
            table.Rows.Add(70, 15);
            table.Rows.Add(71, 25);
            table.Rows.Add(72, 15);
            table.Rows.Add(73, 10);
            table.Rows.Add(74, 20);
            table.Rows.Add(75, 25);
            table.Rows.Add(76, 10);
            table.Rows.Add(77, 35);
            table.Rows.Add(78, 30);
            table.Rows.Add(79, 15);
            table.Rows.Add(80, 10);
            table.Rows.Add(81, 40);
            table.Rows.Add(82, 10);
            table.Rows.Add(83, 15);
            table.Rows.Add(84, 30);
            table.Rows.Add(85, 15);
            table.Rows.Add(86, 20);
            table.Rows.Add(87, 10);
            table.Rows.Add(88, 15);
            table.Rows.Add(89, 10);
            table.Rows.Add(90, 5);
            table.Rows.Add(91, 10);
            table.Rows.Add(92, 10);
            table.Rows.Add(93, 25);
            table.Rows.Add(94, 10);
            table.Rows.Add(95, 20);
            table.Rows.Add(96, 40);
            table.Rows.Add(97, 30);
            table.Rows.Add(98, 30);
            table.Rows.Add(99, 20);
            table.Rows.Add(100, 20);
            table.Rows.Add(101, 15);
            table.Rows.Add(102, 10);
            table.Rows.Add(103, 40);
            table.Rows.Add(104, 15);
            table.Rows.Add(105, 10);
            table.Rows.Add(106, 30);
            table.Rows.Add(107, 10);
            table.Rows.Add(108, 20);
            table.Rows.Add(109, 10);
            table.Rows.Add(110, 40);
            table.Rows.Add(111, 40);
            table.Rows.Add(112, 20);
            table.Rows.Add(113, 30);
            table.Rows.Add(114, 30);
            table.Rows.Add(115, 20);
            table.Rows.Add(116, 30);
            table.Rows.Add(117, 10);
            table.Rows.Add(118, 10);
            table.Rows.Add(119, 20);
            table.Rows.Add(120, 5);
            table.Rows.Add(121, 10);
            table.Rows.Add(122, 30);
            table.Rows.Add(123, 20);
            table.Rows.Add(124, 20);
            table.Rows.Add(125, 20);
            table.Rows.Add(126, 5);
            table.Rows.Add(127, 15);
            table.Rows.Add(128, 10);
            table.Rows.Add(129, 20);
            table.Rows.Add(130, 10);
            table.Rows.Add(131, 15);
            table.Rows.Add(132, 35);
            table.Rows.Add(133, 20);
            table.Rows.Add(134, 15);
            table.Rows.Add(135, 10);
            table.Rows.Add(136, 10);
            table.Rows.Add(137, 30);
            table.Rows.Add(138, 15);
            table.Rows.Add(139, 40);
            table.Rows.Add(140, 20);
            table.Rows.Add(141, 15);
            table.Rows.Add(142, 10);
            table.Rows.Add(143, 5);
            table.Rows.Add(144, 10);
            table.Rows.Add(145, 30);
            table.Rows.Add(146, 10);
            table.Rows.Add(147, 15);
            table.Rows.Add(148, 20);
            table.Rows.Add(149, 15);
            table.Rows.Add(150, 40);
            table.Rows.Add(151, 20);
            table.Rows.Add(152, 10);
            table.Rows.Add(153, 5);
            table.Rows.Add(154, 15);
            table.Rows.Add(155, 10);
            table.Rows.Add(156, 10);
            table.Rows.Add(157, 10);
            table.Rows.Add(158, 15);
            table.Rows.Add(159, 30);
            table.Rows.Add(160, 30);
            table.Rows.Add(161, 10);
            table.Rows.Add(162, 10);
            table.Rows.Add(163, 20);
            table.Rows.Add(164, 10);
            table.Rows.Add(165, 1);
            table.Rows.Add(166, 1);
            table.Rows.Add(167, 10);
            table.Rows.Add(168, 25);
            table.Rows.Add(169, 10);
            table.Rows.Add(170, 5);
            table.Rows.Add(171, 15);
            table.Rows.Add(172, 25);
            table.Rows.Add(173, 15);
            table.Rows.Add(174, 10);
            table.Rows.Add(175, 15);
            table.Rows.Add(176, 30);
            table.Rows.Add(177, 5);
            table.Rows.Add(178, 40);
            table.Rows.Add(179, 15);
            table.Rows.Add(180, 10);
            table.Rows.Add(181, 25);
            table.Rows.Add(182, 10);
            table.Rows.Add(183, 30);
            table.Rows.Add(184, 10);
            table.Rows.Add(185, 20);
            table.Rows.Add(186, 10);
            table.Rows.Add(187, 10);
            table.Rows.Add(188, 10);
            table.Rows.Add(189, 10);
            table.Rows.Add(190, 10);
            table.Rows.Add(191, 20);
            table.Rows.Add(192, 5);
            table.Rows.Add(193, 40);
            table.Rows.Add(194, 5);
            table.Rows.Add(195, 5);
            table.Rows.Add(196, 15);
            table.Rows.Add(197, 5);
            table.Rows.Add(198, 10);
            table.Rows.Add(199, 5);
            table.Rows.Add(200, 10);
            table.Rows.Add(201, 10);
            table.Rows.Add(202, 10);
            table.Rows.Add(203, 10);
            table.Rows.Add(204, 20);
            table.Rows.Add(205, 20);
            table.Rows.Add(206, 40);
            table.Rows.Add(207, 15);
            table.Rows.Add(208, 10);
            table.Rows.Add(209, 20);
            table.Rows.Add(210, 20);
            table.Rows.Add(211, 25);
            table.Rows.Add(212, 5);
            table.Rows.Add(213, 15);
            table.Rows.Add(214, 10);
            table.Rows.Add(215, 5);
            table.Rows.Add(216, 20);
            table.Rows.Add(217, 15);
            table.Rows.Add(218, 20);
            table.Rows.Add(219, 25);
            table.Rows.Add(220, 20);
            table.Rows.Add(221, 5);
            table.Rows.Add(222, 30);
            table.Rows.Add(223, 5);
            table.Rows.Add(224, 10);
            table.Rows.Add(225, 20);
            table.Rows.Add(226, 40);
            table.Rows.Add(227, 5);
            table.Rows.Add(228, 20);
            table.Rows.Add(229, 40);
            table.Rows.Add(230, 20);
            table.Rows.Add(231, 15);
            table.Rows.Add(232, 35);
            table.Rows.Add(233, 10);
            table.Rows.Add(234, 5);
            table.Rows.Add(235, 5);
            table.Rows.Add(236, 5);
            table.Rows.Add(237, 15);
            table.Rows.Add(238, 5);
            table.Rows.Add(239, 20);
            table.Rows.Add(240, 5);
            table.Rows.Add(241, 5);
            table.Rows.Add(242, 15);
            table.Rows.Add(243, 20);
            table.Rows.Add(244, 10);
            table.Rows.Add(245, 5);
            table.Rows.Add(246, 5);
            table.Rows.Add(247, 15);
            table.Rows.Add(248, 10);
            table.Rows.Add(249, 15);
            table.Rows.Add(250, 15);
            table.Rows.Add(251, 10);
            table.Rows.Add(252, 10);
            table.Rows.Add(253, 10);
            table.Rows.Add(254, 20);
            table.Rows.Add(255, 10);
            table.Rows.Add(256, 10);
            table.Rows.Add(257, 10);
            table.Rows.Add(258, 10);
            table.Rows.Add(259, 15);
            table.Rows.Add(260, 15);
            table.Rows.Add(261, 15);
            table.Rows.Add(262, 10);
            table.Rows.Add(263, 20);
            table.Rows.Add(264, 20);
            table.Rows.Add(265, 10);
            table.Rows.Add(266, 20);
            table.Rows.Add(267, 20);
            table.Rows.Add(268, 20);
            table.Rows.Add(269, 20);
            table.Rows.Add(270, 20);
            table.Rows.Add(271, 10);
            table.Rows.Add(272, 10);
            table.Rows.Add(273, 10);
            table.Rows.Add(274, 20);
            table.Rows.Add(275, 20);
            table.Rows.Add(276, 5);
            table.Rows.Add(277, 15);
            table.Rows.Add(278, 10);
            table.Rows.Add(279, 10);
            table.Rows.Add(280, 15);
            table.Rows.Add(281, 10);
            table.Rows.Add(282, 20);
            table.Rows.Add(283, 5);
            table.Rows.Add(284, 5);
            table.Rows.Add(285, 10);
            table.Rows.Add(286, 10);
            table.Rows.Add(287, 20);
            table.Rows.Add(288, 5);
            table.Rows.Add(289, 10);
            table.Rows.Add(290, 20);
            table.Rows.Add(291, 10);
            table.Rows.Add(292, 20);
            table.Rows.Add(293, 20);
            table.Rows.Add(294, 20);
            table.Rows.Add(295, 5);
            table.Rows.Add(296, 5);
            table.Rows.Add(297, 15);
            table.Rows.Add(298, 20);
            table.Rows.Add(299, 10);
            table.Rows.Add(300, 15);
            table.Rows.Add(301, 20);
            table.Rows.Add(302, 15);
            table.Rows.Add(303, 10);
            table.Rows.Add(304, 10);
            table.Rows.Add(305, 15);
            table.Rows.Add(306, 10);
            table.Rows.Add(307, 5);
            table.Rows.Add(308, 5);
            table.Rows.Add(309, 10);
            table.Rows.Add(310, 15);
            table.Rows.Add(311, 10);
            table.Rows.Add(312, 5);
            table.Rows.Add(313, 20);
            table.Rows.Add(314, 25);
            table.Rows.Add(315, 5);
            table.Rows.Add(316, 40);
            table.Rows.Add(317, 15);
            table.Rows.Add(318, 5);
            table.Rows.Add(319, 40);
            table.Rows.Add(320, 15);
            table.Rows.Add(321, 20);
            table.Rows.Add(322, 20);
            table.Rows.Add(323, 5);
            table.Rows.Add(324, 15);
            table.Rows.Add(325, 20);
            table.Rows.Add(326, 20);
            table.Rows.Add(327, 15);
            table.Rows.Add(328, 15);
            table.Rows.Add(329, 5);
            table.Rows.Add(330, 10);
            table.Rows.Add(331, 30);
            table.Rows.Add(332, 20);
            table.Rows.Add(333, 30);
            table.Rows.Add(334, 15);
            table.Rows.Add(335, 5);
            table.Rows.Add(336, 40);
            table.Rows.Add(337, 15);
            table.Rows.Add(338, 5);
            table.Rows.Add(339, 20);
            table.Rows.Add(340, 5);
            table.Rows.Add(341, 15);
            table.Rows.Add(342, 25);
            table.Rows.Add(343, 25);
            table.Rows.Add(344, 15);
            table.Rows.Add(345, 20);
            table.Rows.Add(346, 15);
            table.Rows.Add(347, 20);
            table.Rows.Add(348, 15);
            table.Rows.Add(349, 20);
            table.Rows.Add(350, 10);
            table.Rows.Add(351, 20);
            table.Rows.Add(352, 20);
            table.Rows.Add(353, 5);
            table.Rows.Add(354, 5);
            table.Rows.Add(355, 10);
            table.Rows.Add(356, 5);
            table.Rows.Add(357, 40);
            table.Rows.Add(358, 10);
            table.Rows.Add(359, 10);
            table.Rows.Add(360, 5);
            table.Rows.Add(361, 10);
            table.Rows.Add(362, 10);
            table.Rows.Add(363, 15);
            table.Rows.Add(364, 10);
            table.Rows.Add(365, 20);
            table.Rows.Add(366, 15);
            table.Rows.Add(367, 30);
            table.Rows.Add(368, 10);
            table.Rows.Add(369, 20);
            table.Rows.Add(370, 5);
            table.Rows.Add(371, 10);
            table.Rows.Add(372, 10);
            table.Rows.Add(373, 15);
            table.Rows.Add(374, 10);
            table.Rows.Add(375, 10);
            table.Rows.Add(376, 5);
            table.Rows.Add(377, 15);
            table.Rows.Add(378, 5);
            table.Rows.Add(379, 10);
            table.Rows.Add(380, 10);
            table.Rows.Add(381, 30);
            table.Rows.Add(382, 20);
            table.Rows.Add(383, 20);
            table.Rows.Add(384, 10);
            table.Rows.Add(385, 10);
            table.Rows.Add(386, 5);
            table.Rows.Add(387, 5);
            table.Rows.Add(388, 10);
            table.Rows.Add(389, 5);
            table.Rows.Add(390, 20);
            table.Rows.Add(391, 10);
            table.Rows.Add(392, 20);
            table.Rows.Add(393, 10);
            table.Rows.Add(394, 15);
            table.Rows.Add(395, 10);
            table.Rows.Add(396, 20);
            table.Rows.Add(397, 20);
            table.Rows.Add(398, 20);
            table.Rows.Add(399, 15);
            table.Rows.Add(400, 15);
            table.Rows.Add(401, 10);
            table.Rows.Add(402, 15);
            table.Rows.Add(403, 15);
            table.Rows.Add(404, 15);
            table.Rows.Add(405, 10);
            table.Rows.Add(406, 10);
            table.Rows.Add(407, 10);
            table.Rows.Add(408, 20);
            table.Rows.Add(409, 10);
            table.Rows.Add(410, 30);
            table.Rows.Add(411, 5);
            table.Rows.Add(412, 10);
            table.Rows.Add(413, 15);
            table.Rows.Add(414, 10);
            table.Rows.Add(415, 10);
            table.Rows.Add(416, 5);
            table.Rows.Add(417, 20);
            table.Rows.Add(418, 30);
            table.Rows.Add(419, 10);
            table.Rows.Add(420, 30);
            table.Rows.Add(421, 15);
            table.Rows.Add(422, 15);
            table.Rows.Add(423, 15);
            table.Rows.Add(424, 15);
            table.Rows.Add(425, 30);
            table.Rows.Add(426, 10);
            table.Rows.Add(427, 20);
            table.Rows.Add(428, 15);
            table.Rows.Add(429, 10);
            table.Rows.Add(430, 10);
            table.Rows.Add(431, 20);
            table.Rows.Add(432, 15);
            table.Rows.Add(433, 5);
            table.Rows.Add(434, 5);
            table.Rows.Add(435, 15);
            table.Rows.Add(436, 15);
            table.Rows.Add(437, 5);
            table.Rows.Add(438, 10);
            table.Rows.Add(439, 5);
            table.Rows.Add(440, 20);
            table.Rows.Add(441, 5);
            table.Rows.Add(442, 15);
            table.Rows.Add(443, 20);
            table.Rows.Add(444, 5);
            table.Rows.Add(445, 20);
            table.Rows.Add(446, 20);
            table.Rows.Add(447, 20);
            table.Rows.Add(448, 20);
            table.Rows.Add(449, 10);
            table.Rows.Add(450, 20);
            table.Rows.Add(451, 10);
            table.Rows.Add(452, 15);
            table.Rows.Add(453, 20);
            table.Rows.Add(454, 15);
            table.Rows.Add(455, 10);
            table.Rows.Add(456, 10);
            table.Rows.Add(457, 5);
            table.Rows.Add(458, 10);
            table.Rows.Add(459, 5);
            table.Rows.Add(460, 5);
            table.Rows.Add(461, 10);
            table.Rows.Add(462, 5);
            table.Rows.Add(463, 5);
            table.Rows.Add(464, 10);
            table.Rows.Add(465, 5);
            table.Rows.Add(466, 5);
            table.Rows.Add(467, 5);
            table.Rows.Add(468, 15);
            table.Rows.Add(469, 10);
            table.Rows.Add(470, 10);
            table.Rows.Add(471, 10);
            table.Rows.Add(472, 10);
            table.Rows.Add(473, 10);
            table.Rows.Add(474, 10);
            table.Rows.Add(475, 15);
            table.Rows.Add(476, 20);
            table.Rows.Add(477, 15);
            table.Rows.Add(478, 10);
            table.Rows.Add(479, 15);
            table.Rows.Add(480, 10);
            table.Rows.Add(481, 15);
            table.Rows.Add(482, 10);
            table.Rows.Add(483, 20);
            table.Rows.Add(484, 10);
            table.Rows.Add(485, 15);
            table.Rows.Add(486, 10);
            table.Rows.Add(487, 20);
            table.Rows.Add(488, 20);
            table.Rows.Add(489, 20);
            table.Rows.Add(490, 20);
            table.Rows.Add(491, 20);
            table.Rows.Add(492, 15);
            table.Rows.Add(493, 15);
            table.Rows.Add(494, 15);
            table.Rows.Add(495, 15);
            table.Rows.Add(496, 15);
            table.Rows.Add(497, 15);
            table.Rows.Add(498, 20);
            table.Rows.Add(499, 15);
            table.Rows.Add(500, 10);
            table.Rows.Add(501, 15);
            table.Rows.Add(502, 15);
            table.Rows.Add(503, 15);
            table.Rows.Add(504, 15);
            table.Rows.Add(505, 10);
            table.Rows.Add(506, 10);
            table.Rows.Add(507, 10);
            table.Rows.Add(508, 10);
            table.Rows.Add(509, 10);
            table.Rows.Add(510, 15);
            table.Rows.Add(511, 15);
            table.Rows.Add(512, 15);
            table.Rows.Add(513, 15);
            table.Rows.Add(514, 5);
            table.Rows.Add(515, 5);
            table.Rows.Add(516, 15);
            table.Rows.Add(517, 5);
            table.Rows.Add(518, 10);
            table.Rows.Add(519, 10);
            table.Rows.Add(520, 10);
            table.Rows.Add(521, 20);
            table.Rows.Add(522, 20);
            table.Rows.Add(523, 20);
            table.Rows.Add(524, 10);
            table.Rows.Add(525, 10);
            table.Rows.Add(526, 30);
            table.Rows.Add(527, 15);
            table.Rows.Add(528, 15);
            table.Rows.Add(529, 10);
            table.Rows.Add(530, 15);
            table.Rows.Add(531, 25);
            table.Rows.Add(532, 10);
            table.Rows.Add(533, 15);
            table.Rows.Add(534, 10);
            table.Rows.Add(535, 10);
            table.Rows.Add(536, 10);
            table.Rows.Add(537, 20);
            table.Rows.Add(538, 10);
            table.Rows.Add(539, 10);
            table.Rows.Add(540, 10);
            table.Rows.Add(541, 10);
            table.Rows.Add(542, 10);
            table.Rows.Add(543, 15);
            table.Rows.Add(544, 15);
            table.Rows.Add(545, 5);
            table.Rows.Add(546, 5);
            table.Rows.Add(547, 10);
            table.Rows.Add(548, 10);
            table.Rows.Add(549, 10);
            table.Rows.Add(550, 5);
            table.Rows.Add(551, 5);
            table.Rows.Add(552, 10);
            table.Rows.Add(553, 5);
            table.Rows.Add(554, 5);
            table.Rows.Add(555, 15);
            table.Rows.Add(556, 10);
            table.Rows.Add(557, 5);
            table.Rows.Add(558, 5);
            table.Rows.Add(559, 5);
            table.Rows.Add(560, 10);
            table.Rows.Add(561, 10);
            table.Rows.Add(562, 10);
            table.Rows.Add(563, 10);
            table.Rows.Add(564, 20);
            table.Rows.Add(565, 25);
            table.Rows.Add(566, 10);
            table.Rows.Add(567, 20);
            table.Rows.Add(568, 30);
            table.Rows.Add(569, 25);
            table.Rows.Add(570, 20);
            table.Rows.Add(571, 20);
            table.Rows.Add(572, 15);
            table.Rows.Add(573, 20);
            table.Rows.Add(574, 15);
            table.Rows.Add(575, 20);
            table.Rows.Add(576, 20);
            table.Rows.Add(577, 10);
            table.Rows.Add(578, 10);
            table.Rows.Add(579, 10);
            table.Rows.Add(580, 10);
            table.Rows.Add(581, 10);
            table.Rows.Add(582, 20);
            table.Rows.Add(583, 10);
            table.Rows.Add(584, 30);
            table.Rows.Add(585, 15);
            table.Rows.Add(586, 10);
            table.Rows.Add(587, 10);
            table.Rows.Add(588, 10);
            table.Rows.Add(589, 20);
            table.Rows.Add(590, 20);
            table.Rows.Add(591, 5);
            table.Rows.Add(592, 5);
            table.Rows.Add(593, 5);
            table.Rows.Add(594, 20);
            table.Rows.Add(595, 10);
            table.Rows.Add(596, 10);
            table.Rows.Add(597, 20);
            table.Rows.Add(598, 15);
            table.Rows.Add(599, 20);
            table.Rows.Add(600, 20);
            table.Rows.Add(601, 10);
            table.Rows.Add(602, 20);
            table.Rows.Add(603, 30);
            table.Rows.Add(604, 10);
            table.Rows.Add(605, 10);
            table.Rows.Add(606, 40);
            table.Rows.Add(607, 40);
            table.Rows.Add(608, 30);
            table.Rows.Add(609, 20);
            table.Rows.Add(610, 40);
            table.Rows.Add(611, 20);
            table.Rows.Add(612, 20);
            table.Rows.Add(613, 10);
            table.Rows.Add(614, 10);
            table.Rows.Add(615, 10);
            table.Rows.Add(616, 10);
            table.Rows.Add(617, 5);
            table.Rows.Add(618, 10);
            table.Rows.Add(619, 10);
            table.Rows.Add(620, 5);
            table.Rows.Add(621, 5);

            return table;
        }
        public static DataTable ExpTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Level", typeof(int));

            table.Columns.Add("0 - Erratic", typeof(uint));
            table.Columns.Add("1 - Fast", typeof(uint));
            table.Columns.Add("2 - MF", typeof(uint));
            table.Columns.Add("3 - MS", typeof(uint));
            table.Columns.Add("4 - Slow", typeof(uint));
            table.Columns.Add("5 - Fluctuating", typeof(uint));
            table.Rows.Add(0, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(1, 0, 0, 0, 0, 0, 0);
            table.Rows.Add(2, 15, 6, 8, 9, 10, 4);
            table.Rows.Add(3, 52, 21, 27, 57, 33, 13);
            table.Rows.Add(4, 122, 51, 64, 96, 80, 32);
            table.Rows.Add(5, 237, 100, 125, 135, 156, 65);
            table.Rows.Add(6, 406, 172, 216, 179, 270, 112);
            table.Rows.Add(7, 637, 274, 343, 236, 428, 178);
            table.Rows.Add(8, 942, 409, 512, 314, 640, 276);
            table.Rows.Add(9, 1326, 583, 729, 419, 911, 393);
            table.Rows.Add(10, 1800, 800, 1000, 560, 1250, 540);
            table.Rows.Add(11, 2369, 1064, 1331, 742, 1663, 745);
            table.Rows.Add(12, 3041, 1382, 1728, 973, 2160, 967);
            table.Rows.Add(13, 3822, 1757, 2197, 1261, 2746, 1230);
            table.Rows.Add(14, 4719, 2195, 2744, 1612, 3430, 1591);
            table.Rows.Add(15, 5737, 2700, 3375, 2035, 4218, 1957);
            table.Rows.Add(16, 6881, 3276, 4096, 2535, 5120, 2457);
            table.Rows.Add(17, 8155, 3930, 4913, 3120, 6141, 3046);
            table.Rows.Add(18, 9564, 4665, 5832, 3798, 7290, 3732);
            table.Rows.Add(19, 11111, 5487, 6859, 4575, 8573, 4526);
            table.Rows.Add(20, 12800, 6400, 8000, 5460, 10000, 5440);
            table.Rows.Add(21, 14632, 7408, 9261, 6458, 11576, 6482);
            table.Rows.Add(22, 16610, 8518, 10648, 7577, 13310, 7666);
            table.Rows.Add(23, 18737, 9733, 12167, 8825, 15208, 9003);
            table.Rows.Add(24, 21012, 11059, 13824, 10208, 17280, 10506);
            table.Rows.Add(25, 23437, 12500, 15625, 11735, 19531, 12187);
            table.Rows.Add(26, 26012, 14060, 17576, 13411, 21970, 14060);
            table.Rows.Add(27, 28737, 15746, 19683, 15244, 24603, 16140);
            table.Rows.Add(28, 31610, 17561, 21952, 17242, 27440, 18439);
            table.Rows.Add(29, 34632, 19511, 24389, 19411, 30486, 20974);
            table.Rows.Add(30, 37800, 21600, 27000, 21760, 33750, 23760);
            table.Rows.Add(31, 41111, 23832, 29791, 24294, 37238, 26811);
            table.Rows.Add(32, 44564, 26214, 32768, 27021, 40960, 30146);
            table.Rows.Add(33, 48155, 28749, 35937, 29949, 44921, 33780);
            table.Rows.Add(34, 51881, 31443, 39304, 33084, 49130, 37731);
            table.Rows.Add(35, 55737, 34300, 42875, 36435, 53593, 42017);
            table.Rows.Add(36, 59719, 37324, 46656, 40007, 58320, 46656);
            table.Rows.Add(37, 63822, 40522, 50653, 43808, 63316, 50653);
            table.Rows.Add(38, 68041, 43897, 54872, 47846, 68590, 55969);
            table.Rows.Add(39, 72369, 47455, 59319, 52127, 74148, 60505);
            table.Rows.Add(40, 76800, 51200, 64000, 56660, 80000, 66560);
            table.Rows.Add(41, 81326, 55136, 68921, 61450, 86151, 71677);
            table.Rows.Add(42, 85942, 59270, 74088, 66505, 92610, 78533);
            table.Rows.Add(43, 90637, 63605, 79507, 71833, 99383, 84277);
            table.Rows.Add(44, 95406, 68147, 85184, 77440, 106480, 91998);
            table.Rows.Add(45, 100237, 72900, 91125, 83335, 113906, 98415);
            table.Rows.Add(46, 105122, 77868, 97336, 89523, 121670, 107069);
            table.Rows.Add(47, 110052, 83058, 103823, 96012, 129778, 114205);
            table.Rows.Add(48, 115015, 88473, 110592, 102810, 138240, 123863);
            table.Rows.Add(49, 120001, 94119, 117649, 109923, 147061, 131766);
            table.Rows.Add(50, 125000, 100000, 125000, 117360, 156250, 142500);
            table.Rows.Add(51, 131324, 106120, 132651, 125126, 165813, 151222);
            table.Rows.Add(52, 137795, 112486, 140608, 133229, 175760, 163105);
            table.Rows.Add(53, 144410, 119101, 148877, 141677, 186096, 172697);
            table.Rows.Add(54, 151165, 125971, 157464, 150476, 196830, 185807);
            table.Rows.Add(55, 158056, 133100, 166375, 159635, 207968, 196322);
            table.Rows.Add(56, 165079, 140492, 175616, 169159, 219520, 210739);
            table.Rows.Add(57, 172229, 148154, 185193, 179056, 231491, 222231);
            table.Rows.Add(58, 179503, 156089, 195112, 189334, 243890, 238036);
            table.Rows.Add(59, 186894, 164303, 205379, 199999, 256723, 250562);
            table.Rows.Add(60, 194400, 172800, 216000, 211060, 270000, 267840);
            table.Rows.Add(61, 202013, 181584, 226981, 222522, 283726, 281456);
            table.Rows.Add(62, 209728, 190662, 238328, 234393, 297910, 300293);
            table.Rows.Add(63, 217540, 200037, 250047, 246681, 312558, 315059);
            table.Rows.Add(64, 225443, 209715, 262144, 259392, 327680, 335544);
            table.Rows.Add(65, 233431, 219700, 274625, 272535, 343281, 351520);
            table.Rows.Add(66, 241496, 229996, 287496, 286115, 359370, 373744);
            table.Rows.Add(67, 249633, 240610, 300763, 300140, 375953, 390991);
            table.Rows.Add(68, 257834, 251545, 314432, 314618, 393040, 415050);
            table.Rows.Add(69, 267406, 262807, 328509, 329555, 410636, 433631);
            table.Rows.Add(70, 276458, 274400, 343000, 344960, 428750, 459620);
            table.Rows.Add(71, 286328, 286328, 357911, 360838, 447388, 479600);
            table.Rows.Add(72, 296358, 298598, 373248, 377197, 466560, 507617);
            table.Rows.Add(73, 305767, 311213, 389017, 394045, 486271, 529063);
            table.Rows.Add(74, 316074, 324179, 405224, 411388, 506530, 559209);
            table.Rows.Add(75, 326531, 337500, 421875, 429235, 527343, 582187);
            table.Rows.Add(76, 336255, 351180, 438976, 447591, 548720, 614566);
            table.Rows.Add(77, 346965, 365226, 456533, 466464, 570666, 639146);
            table.Rows.Add(78, 357812, 379641, 474552, 485862, 593190, 673863);
            table.Rows.Add(79, 367807, 394431, 493039, 505791, 616298, 700115);
            table.Rows.Add(80, 378880, 409600, 512000, 526260, 640000, 737280);
            table.Rows.Add(81, 390077, 425152, 531441, 547274, 664301, 765275);
            table.Rows.Add(82, 400293, 441094, 551368, 568841, 689210, 804997);
            table.Rows.Add(83, 411686, 457429, 571787, 590969, 714733, 834809);
            table.Rows.Add(84, 423190, 474163, 592704, 613664, 740880, 877201);
            table.Rows.Add(85, 433572, 491300, 614125, 636935, 767656, 908905);
            table.Rows.Add(86, 445239, 508844, 636056, 660787, 795070, 954084);
            table.Rows.Add(87, 457001, 526802, 658503, 685228, 823128, 987754);
            table.Rows.Add(88, 467489, 545177, 681472, 710266, 851840, 1035837);
            table.Rows.Add(89, 479378, 563975, 704969, 735907, 881211, 1071552);
            table.Rows.Add(90, 491346, 583200, 729000, 762160, 911250, 1122660);
            table.Rows.Add(91, 501878, 602856, 753571, 789030, 941963, 1160499);
            table.Rows.Add(92, 513934, 622950, 778688, 816525, 973360, 1214753);
            table.Rows.Add(93, 526049, 643485, 804357, 844653, 1005446, 1254796);
            table.Rows.Add(94, 536557, 664467, 830584, 873420, 1038230, 1312322);
            table.Rows.Add(95, 548720, 685900, 857375, 902835, 1071718, 1354652);
            table.Rows.Add(96, 560922, 707788, 884736, 932903, 1105920, 1415577);
            table.Rows.Add(97, 571333, 730138, 912673, 963632, 1140841, 1460276);
            table.Rows.Add(98, 583539, 752953, 941192, 995030, 1176490, 1524731);
            table.Rows.Add(99, 591882, 776239, 970299, 1027103, 1212873, 1571884);
            table.Rows.Add(100, 600000, 800000, 1000000, 1059860, 1250000, 1640000);
            return table;
        }

        // Stat Fetching
        public static int getMovePP(int move, int ppup)
        {
            return (getBasePP(move) * (5 + ppup) / 5);
        }
        public static int getBasePP(int move)
        {
            if (move < 0) { move = 0; }
            DataTable movepptable = MovePPTable();
            return (int)movepptable.Rows[move][1];
        }
        public static byte[] getRandomEVs()
        {
            byte[] evs = new Byte[6];
          start:
            evs[0] = (byte)Math.Min(Util.rnd32() % 300, 252); // bias two to get maybe 252
            evs[1] = (byte)Math.Min(Util.rnd32() % 300, 252);
            evs[2] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1])), 252);
            evs[3] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1] - evs[2])), 252);
            evs[4] = (byte)Math.Min(((Util.rnd32()) % (510 - evs[0] - evs[1] - evs[2] - evs[3])), 252);
            evs[5] = (byte)Math.Min((510 - evs[0] - evs[1] - evs[2] - evs[3] - evs[4]), 252);
            Util.Shuffle(evs);
            if (evs.Sum(b => (ushort)b) > 510) goto start; // try again!
            return evs;
        }
        public static byte getBaseFriendship(int species)
        {
            PersonalParser.Personal Mon = PersonalGetter.GetPersonal(species);
            return Mon.BaseFriendship;
        }
        public static int getLevel(int species, ref uint exp)
        {
            if (exp == 0) { return 1; }
            int tl = 1; // Initial Level

            DataTable table = PKX.ExpTable();
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);

            int growth = MonData.EXPGrowth;

            if ((uint)table.Rows[tl][growth + 1] < exp)
            {
                while ((uint)table.Rows[tl][growth + 1] < exp)
                {
                    // While EXP for guessed level is below our current exp
                    tl += 1;
                    if (tl == 100)
                    {
                        exp = getEXP(100, species);
                        return tl;
                    }
                    // when calcexp exceeds our exp, we exit loop
                }
                if ((uint)table.Rows[tl][growth + 1] == exp) // Matches level threshold
                    return tl;
                else return (tl - 1);
            }
            else return tl;
        }
        public static uint getEXP(int level, int species)
        {
            // Fetch Growth
            if ((level == 0) || (level == 1))
                return 0;
            if (level > 100) level = 100;

            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int growth = MonData.EXPGrowth;

            uint exp = (uint)PKX.ExpTable().Rows[level][growth+1];
            return exp;
        }
        public static int[] getAbilities(int species, int formnum)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species, formnum);
            int[] abils = { 0, 0, 0 };
            Array.Copy(MonData.Abilities, abils, 3);
            return abils;
        }
        public static int getGender(string s)
        {
            if (s == "♂" || s == "M")
                return 0;
            else if (s == "♀" || s == "F")
                return 1;
            else return 2;
        }
        public static ushort[] getStats(int species, int level, int nature, int form,
                                        int HP_EV, int ATK_EV, int DEF_EV, int SPA_EV, int SPD_EV, int SPE_EV,
                                        int HP_IV, int ATK_IV, int DEF_IV, int SPA_IV, int SPD_IV, int SPE_IV)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species, form);
            int HP_B = MonData.BaseStats[0];
            int ATK_B = MonData.BaseStats[1];
            int DEF_B = MonData.BaseStats[2];
            int SPE_B = MonData.BaseStats[3];
            int SPA_B = MonData.BaseStats[4];
            int SPD_B = MonData.BaseStats[5];
            
            // Calculate Stats
            ushort[] stats = new ushort[6]; // Stats are stored as ushorts in the PKX structure. We'll cap them as such.
            stats[0] = (ushort)((((HP_IV + (2 * HP_B) + (HP_EV / 4) + 100) * level) / 100) + 10);
            stats[1] = (ushort)((((ATK_IV + (2 * ATK_B) + (ATK_EV / 4)) * level) / 100) + 5);
            stats[2] = (ushort)((((DEF_IV + (2 * DEF_B) + (DEF_EV / 4)) * level) / 100) + 5);
            stats[4] = (ushort)((((SPA_IV + (2 * SPA_B) + (SPA_EV / 4)) * level) / 100) + 5);
            stats[5] = (ushort)((((SPD_IV + (2 * SPD_B) + (SPD_EV / 4)) * level) / 100) + 5);
            stats[3] = (ushort)((((SPE_IV + (2 * SPE_B) + (SPE_EV / 4)) * level) / 100) + 5);

            // Account for nature
            int incr = nature / 5 + 1;
            int decr = nature % 5 + 1;
            if (incr != decr)
            {
                stats[incr] *= 11; stats[incr] /= 10;
                stats[decr] *=  9; stats[decr] /= 10;
            }

            // Return Result
            return stats;
        }

        // Manipulation
        public static byte[] shuffleArray(byte[] pkx, uint sv)
        {
            byte[] ekx = new Byte[260];
            Array.Copy(pkx, ekx, 8);

            // Now to shuffle the blocks

            // Define Shuffle Order Structure
            byte[] aloc = new byte[] { 0, 0, 0, 0, 0, 0, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3, 1, 1, 2, 3, 2, 3 };
            byte[] bloc = new byte[] { 1, 1, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2 };
            byte[] cloc = new byte[] { 2, 3, 1, 1, 3, 2, 2, 3, 1, 1, 3, 2, 0, 0, 0, 0, 0, 0, 3, 2, 3, 2, 1, 1 };
            byte[] dloc = new byte[] { 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 3, 2, 3, 2, 1, 1, 0, 0, 0, 0, 0, 0 };

            // Get Shuffle Order
            byte[] shlog = new byte[] { aloc[sv], bloc[sv], cloc[sv], dloc[sv] };

            // UnShuffle Away!
            for (int b = 0; b < 4; b++)
                Array.Copy(pkx, 8 + 56 * shlog[b], ekx, 8 + 56 * b, 56);

            // Fill the Battle Stats back
            if (pkx.Length > 232)
                Array.Copy(pkx, 232, ekx, 232, 28);

            return ekx;
        }
        public static byte[] decryptArray(byte[] ekx)
        {
            byte[] pkx = new Byte[0x104];
            Array.Copy(ekx, pkx, ekx.Length);
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            uint seed = pv;

            // Decrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
            {
                int pre = pkx[i] + ((pkx[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                pkx[i] = (byte)((post) & 0xFF);
                pkx[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Deshuffle
            pkx = shuffleArray(pkx, sv);

            // Decrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
            {
                int pre = pkx[i] + ((pkx[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                pkx[i] = (byte)((post) & 0xFF);
                pkx[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            return pkx;
        }
        public static byte[] encryptArray(byte[] pkx)
        {
            // Shuffle
            uint pv = BitConverter.ToUInt32(pkx, 0);
            uint sv = (((pv & 0x3E000) >> 0xD) % 24);

            byte[] ekxdata = new Byte[pkx.Length];
            Array.Copy(pkx, ekxdata, pkx.Length);

            // If I unshuffle 11 times, the 12th (decryption) will always decrypt to ABCD.
            // 2 x 3 x 4 = 12 (possible unshuffle loops -> total iterations)
            for (int i = 0; i < 11; i++)
                ekxdata = shuffleArray(ekxdata, sv);

            uint seed = pv;
            // Encrypt Blocks with RNG Seed
            for (int i = 8; i < 232; i += 2)
            {
                int pre = ekxdata[i] + ((ekxdata[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                ekxdata[i] = (byte)((post) & 0xFF);
                ekxdata[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Encrypt the Party Stats
            seed = pv;
            for (int i = 232; i < 260; i += 2)
            {
                int pre = ekxdata[i] + ((ekxdata[i + 1]) << 8);
                seed = PKX.LCRNG(seed);
                int seedxor = (int)((seed) >> 16);
                int post = (pre ^ seedxor);
                ekxdata[i] = (byte)((post) & 0xFF);
                ekxdata[i + 1] = (byte)(((post) >> 8) & 0xFF);
            }

            // Done
            return ekxdata;
        }
        public static ushort getCHK(byte[] data)
        {
            ushort chk = 0;
            for (int i = 8; i < 232; i += 2) // Loop through the entire PKX
                chk += BitConverter.ToUInt16(data, i);

            return chk;
        }
        public static bool verifychk(byte[] input)
        {
            ushort checksum = 0;
            if (input.Length == 100 || input.Length == 80)  // Gen 3 Files
            {
                for (int i = 32; i < 80; i += 2)
                    checksum += BitConverter.ToUInt16(input, i);

                return (checksum == BitConverter.ToUInt16(input, 28));
            }
            else
            {
                if (input.Length == 236 || input.Length == 220 || input.Length == 136) // Gen 4/5
                    Array.Resize(ref input, 136);                   
                else if (input.Length == 232 || input.Length == 260) // Gen 6
                    Array.Resize(ref input, 232);                    
                else throw new Exception("Wrong sized input array to verifychecksum");

                ushort chk = 0;
                for (int i = 8; i < input.Length; i += 2)
                    chk += BitConverter.ToUInt16(input, i);

                return (chk == BitConverter.ToUInt16(input, 0x6));
            }
        }
        public static UInt16 getPSV(UInt32 PID)
        {
            return Convert.ToUInt16(((PID >> 16) ^ (PID & 0xFFFF)) >> 4);
        }
        public static UInt16 getTSV(UInt16 TID, UInt16 SID)
        {
            return Convert.ToUInt16((TID ^ SID) >> 4);
        }
        public static uint getRandomPID(int species, int cg)
        {
            PersonalParser.Personal MonData = PersonalGetter.GetPersonal(species);
            int gt = MonData.GenderRatio;
            uint pid = (uint)Util.rnd32();
            if (gt == 255) //Genderless
                return pid;
            if (gt == 254) //Female Only
                gt++;
            while (true) // Loop until we find a suitable PID
            {
                uint gv = (pid & 0xFF);
                if (cg == 2) // Genderless
                    break;  // PID Passes
                else if ((cg == 1) && (gv <= gt)) // Female
                    break;  // PID Passes
                else if ((cg == 0) && (gv > gt))
                    break;  // PID Passes
                pid = (uint)Util.rnd32();
            }
            return pid;
        }

        // Object
        #region PKX Object
        private Image pksprite;
        private uint mEC, mPID, mIV32,

            mexp,
            mHP_EV, mATK_EV, mDEF_EV, mSPA_EV, mSPD_EV, mSPE_EV,
            mHP_IV, mATK_IV, mDEF_IV, mSPE_IV, mSPA_IV, mSPD_IV,
            mcnt_cool, mcnt_beauty, mcnt_cute, mcnt_smart, mcnt_tough, mcnt_sheen,
            mmarkings, mhptype;

        private string
            mnicknamestr, mgenderstring, mnotOT, mot, mSpeciesName, mNatureName, mHPName, mAbilityName,
            mMove1N, mMove2N, mMove3N, mMove4N;

        private int
            mability, mabilitynum, mnature, mfeflag, mgenderflag, maltforms, mPKRS_Strain, mPKRS_Duration,
            mmetlevel, motgender;

        private bool
            misegg, misnick, misshiny;

        private ushort
            mspecies, mhelditem, mTID, mSID, mTSV, mESV,
            mmove1, mmove2, mmove3, mmove4,
            mmove1_pp, mmove2_pp, mmove3_pp, mmove4_pp,
            mmove1_ppu, mmove2_ppu, mmove3_ppu, mmove4_ppu,
            meggmove1, meggmove2, meggmove3, meggmove4,
            mchk,

            mOTfriendship, mOTaffection,
            megg_year, megg_month, megg_day,
            mmet_year, mmet_month, mmet_day,
            meggloc, mmetloc,
            mball, mencountertype,
            mgamevers, mcountryID, mregionID, mdsregID, motlang;

        public Image pkimg { get { return pksprite; } }
        public string Nickname { get { return mnicknamestr; } }
        public string Species { get { return mSpeciesName; } }
        public string Nature { get { return mNatureName; } }
        public string Gender { get { return mgenderstring; } }
        public string ESV { get { return mESV.ToString("0000"); } }
        public string HP_Type { get { return mHPName; } }
        public string Ability { get { return mAbilityName; } }
        public string Move1 { get { return mMove1N; } }
        public string Move2 { get { return mMove2N; } }
        public string Move3 { get { return mMove3N; } }
        public string Move4 { get { return mMove4N; } }

        #region Extraneous
        public string EC { get { return mEC.ToString("X8"); } }
        public string PID { get { return mPID.ToString("X8"); } }
        public uint HP_IV { get { return mHP_IV; } }
        public uint ATK_IV { get { return mATK_IV; } }
        public uint DEF_IV { get { return mDEF_IV; } }
        public uint SPA_IV { get { return mSPA_IV; } }
        public uint SPD_IV { get { return mSPD_IV; } }
        public uint SPE_IV { get { return mSPE_IV; } }
        public uint EXP { get { return mexp; } }
        public uint HP_EV { get { return mHP_EV; } }
        public uint ATK_EV { get { return mATK_EV; } }
        public uint DEF_EV { get { return mDEF_EV; } }
        public uint SPA_EV { get { return mSPA_EV; } }
        public uint SPD_EV { get { return mSPD_EV; } }
        public uint SPE_EV { get { return mSPE_EV; } }
        public uint Cool { get { return mcnt_cool; } }
        public uint Beauty { get { return mcnt_beauty; } }
        public uint Cute { get { return mcnt_cute; } }
        public uint Smart { get { return mcnt_smart; } }
        public uint Tough { get { return mcnt_tough; } }
        public uint Sheen { get { return mcnt_sheen; } }
        public uint Markings { get { return mmarkings; } }

        public string NotOT { get { return mnotOT; } }
        public string OT { get { return mot; } }

        public int AbilityNum { get { return mabilitynum; } }
        public int FatefulFlag { get { return mfeflag; } }
        public int GenderFlag { get { return mgenderflag; } }
        public int AltForms { get { return maltforms; } }
        public int PKRS_Strain { get { return mPKRS_Strain; } }
        public int PKRS_Days { get { return mPKRS_Duration; } }
        public int MetLevel { get { return mmetlevel; } }
        public int OT_Gender { get { return motgender; } }

        public bool IsEgg { get { return misegg; } }
        public bool IsNicknamed { get { return misnick; } }
        public bool IsShiny { get { return misshiny; } }

        public ushort HeldItem { get { return mhelditem; } }
        public ushort TID { get { return mTID; } }
        public ushort SID { get { return mSID; } }
        public ushort TSV { get { return mTSV; } }
        public ushort Move1_PP { get { return mmove1_pp; } }
        public ushort Move2_PP { get { return mmove2_pp; } }
        public ushort Move3_PP { get { return mmove3_pp; } }
        public ushort Move4_PP { get { return mmove4_pp; } }
        public ushort Move1_PPUp { get { return mmove1_ppu; } }
        public ushort Move2_PPUp { get { return mmove2_ppu; } }
        public ushort Move3_PPUp { get { return mmove3_ppu; } }
        public ushort Move4_PPUp { get { return mmove4_ppu; } }
        public ushort EggMove1 { get { return meggmove1; } }
        public ushort EggMove2 { get { return meggmove2; } }
        public ushort EggMove3 { get { return meggmove3; } }
        public ushort EggMove4 { get { return meggmove4; } }
        public ushort Checksum { get { return mchk; } }
        public ushort mFriendship { get { return mOTfriendship; } }
        public ushort OT_Affection { get { return mOTaffection; } }
        public ushort Egg_Year { get { return megg_year; } }
        public ushort Egg_Day { get { return megg_month; } }
        public ushort Egg_Month { get { return megg_day; } }
        public ushort Met_Year { get { return mmet_year; } }
        public ushort Met_Day { get { return mmet_month; } }
        public ushort Met_Month { get { return mmet_day; } }
        public ushort Egg_Location { get { return meggloc; } }
        public ushort Met_Location { get { return mmetloc; } }
        public ushort Ball { get { return mball; } }
        public ushort Encounter { get { return mencountertype; } }
        public ushort GameVersion { get { return mgamevers; } }
        public ushort CountryID { get { return mcountryID; } }
        public ushort RegionID { get { return mregionID; } }
        public ushort DSRegionID { get { return mdsregID; } }
        public ushort OTLang { get { return motlang; } }

        #endregion
        public PKX(byte[] pkx)
        {
            mnicknamestr = "";
            mnotOT = "";
            mot = "";
            mEC = BitConverter.ToUInt32(pkx, 0);
            mchk = BitConverter.ToUInt16(pkx, 6);
            mspecies = BitConverter.ToUInt16(pkx, 0x08);
            mhelditem = BitConverter.ToUInt16(pkx, 0x0A);
            mTID = BitConverter.ToUInt16(pkx, 0x0C);
            mSID = BitConverter.ToUInt16(pkx, 0x0E);
            mexp = BitConverter.ToUInt32(pkx, 0x10);
            mability = pkx[0x14];
            mabilitynum = pkx[0x15];
            // 0x16, 0x17 - unknown
            mPID = BitConverter.ToUInt32(pkx, 0x18);
            mnature = pkx[0x1C];
            mfeflag = pkx[0x1D] % 2;
            mgenderflag = (pkx[0x1D] >> 1) & 0x3;
            maltforms = (pkx[0x1D] >> 3);
            mHP_EV = pkx[0x1E];
            mATK_EV = pkx[0x1F];
            mDEF_EV = pkx[0x20];
            mSPA_EV = pkx[0x22];
            mSPD_EV = pkx[0x23];
            mSPE_EV = pkx[0x21];
            mcnt_cool = pkx[0x24];
            mcnt_beauty = pkx[0x25];
            mcnt_cute = pkx[0x26];
            mcnt_smart = pkx[0x27];
            mcnt_tough = pkx[0x28];
            mcnt_sheen = pkx[0x29];
            mmarkings = pkx[0x2A];
            mPKRS_Strain = pkx[0x2B] >> 4;
            mPKRS_Duration = pkx[0x2B] % 0x10;

            // Block B
            mnicknamestr = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0x40, 24));
            // 0x58, 0x59 - unused
            mmove1 = BitConverter.ToUInt16(pkx, 0x5A);
            mmove2 = BitConverter.ToUInt16(pkx, 0x5C);
            mmove3 = BitConverter.ToUInt16(pkx, 0x5E);
            mmove4 = BitConverter.ToUInt16(pkx, 0x60);
            mmove1_pp = pkx[0x62];
            mmove2_pp = pkx[0x63];
            mmove3_pp = pkx[0x64];
            mmove4_pp = pkx[0x65];
            mmove1_ppu = pkx[0x66];
            mmove2_ppu = pkx[0x67];
            mmove3_ppu = pkx[0x68];
            mmove4_ppu = pkx[0x69];
            meggmove1 = BitConverter.ToUInt16(pkx, 0x6A);
            meggmove2 = BitConverter.ToUInt16(pkx, 0x6C);
            meggmove3 = BitConverter.ToUInt16(pkx, 0x6E);
            meggmove4 = BitConverter.ToUInt16(pkx, 0x70);

            // 0x72 - Super Training Flag - Passed with pkx to new form

            // 0x73 - unused/unknown
            mIV32 = BitConverter.ToUInt32(pkx, 0x74);
            mHP_IV = mIV32 & 0x1F;
            mATK_IV = (mIV32 >> 5) & 0x1F;
            mDEF_IV = (mIV32 >> 10) & 0x1F;
            mSPE_IV = (mIV32 >> 15) & 0x1F;
            mSPA_IV = (mIV32 >> 20) & 0x1F;
            mSPD_IV = (mIV32 >> 25) & 0x1F;
            misegg = Convert.ToBoolean((mIV32 >> 30) & 1);
            misnick = Convert.ToBoolean((mIV32 >> 31));

            // Block C
            mnotOT = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0x78, 24));
            bool notOTG = Convert.ToBoolean(pkx[0x92]);
            // Memory Editor edits everything else with pkx in a new form

            // Block D
            mot = Util.TrimFromZero(Encoding.Unicode.GetString(pkx, 0xB0, 24));
            // 0xC8, 0xC9 - unused
            mOTfriendship = pkx[0xCA];
            mOTaffection = pkx[0xCB]; // Handled by Memory Editor
            // 0xCC, 0xCD, 0xCE, 0xCF, 0xD0
            megg_year = pkx[0xD1];
            megg_month = pkx[0xD2];
            megg_day = pkx[0xD3];
            mmet_year = pkx[0xD4];
            mmet_month = pkx[0xD5];
            mmet_day = pkx[0xD6];
            // 0xD7 - unused
            meggloc = BitConverter.ToUInt16(pkx, 0xD8);
            mmetloc = BitConverter.ToUInt16(pkx, 0xDA);
            mball = pkx[0xDC];
            mmetlevel = pkx[0xDD] & 0x7F;
            motgender = (pkx[0xDD]) >> 7;
            mencountertype = pkx[0xDE];
            mgamevers = pkx[0xDF];
            mcountryID = pkx[0xE0];
            mregionID = pkx[0xE1];
            mdsregID = pkx[0xE2];
            motlang = pkx[0xE3];

            if (mgenderflag == 0)
                mgenderstring = "♂";
            else if (mgenderflag == 1)
                mgenderstring = "♀";
            else 
                mgenderstring = "-";

            mhptype = (15 * ((mHP_IV & 1) + 2 * (mATK_IV & 1) + 4 * (mDEF_IV & 1) + 8 * (mSPE_IV & 1) + 16 * (mSPA_IV & 1) + 32 * (mSPD_IV & 1))) / 63 + 1;

            mTSV = (ushort)((mTID ^ mSID) >> 4);
            mESV = (ushort)(((mPID >> 16) ^ (mPID & 0xFFFF)) >> 4);

            misshiny = (mTSV == mESV);
            // Nidoran Gender Fixing Text
            if (!Convert.ToBoolean(misnick))
            {
                if (mnicknamestr.Contains((char)0xE08F))
                    mnicknamestr = Regex.Replace(mnicknamestr, "\uE08F", "\u2640");
                else if (mnicknamestr.Contains((char)0xE08E))
                    mnicknamestr = Regex.Replace(mnicknamestr, "\uE08E", "\u2642");
            }
            {
                int species = BitConverter.ToInt16(pkx, 0x08); // Get Species
                uint isegg = (BitConverter.ToUInt32(pkx, 0x74) >> 30) & 1;

                int altforms = (pkx[0x1D] >> 3);
                int gender = (pkx[0x1D] >> 1) & 0x3;

                string file;
                if (isegg == 1)
                { file = "egg"; }
                else
                {
                    file = "_" + species.ToString();
                    if (altforms > 0) // Alt Form Handling
                        file = file + "_" + altforms.ToString();
                    else if ((species == 521) && (gender == 1))   // Unfezant
                        file = "_" + species.ToString() + "f";
                }
                if (species == 0)
                    file = "_0";

                pksprite = (Image)Properties.Resources.ResourceManager.GetObject(file);
            }
            try
            {
                mSpeciesName = Form1.specieslist[mspecies];
                mNatureName = Form1.natures[mnature];
                mHPName = Form1.types[mhptype];
                mAbilityName = Form1.abilitylist[mability];
                mMove1N = Form1.movelist[mmove1];
                mMove2N = Form1.movelist[mmove2];
                mMove3N = Form1.movelist[mmove3];
                mMove4N = Form1.movelist[mmove4];
            }
            catch { return; }
        }
        #endregion

        // Save File Related
        internal static int detectSAVIndex(byte[] data, ref int savindex)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            {
                byte[] difihash1 = new Byte[0x12C];
                byte[] difihash2 = new Byte[0x12C];
                Array.Copy(data, 0x330, difihash1, 0, 0x12C);
                Array.Copy(data, 0x200, difihash2, 0, 0x12C);
                byte[] hashValue1 = mySHA256.ComputeHash(difihash1);
                byte[] hashValue2 = mySHA256.ComputeHash(difihash2);
                byte[] actualhash = new Byte[0x20];
                Array.Copy(data, 0x16C, actualhash, 0, 0x20);
                if (hashValue1.SequenceEqual(actualhash))
                {
                    Console.WriteLine("Active DIFI 2 - Save 1.");
                    savindex = 0;
                }
                else if (hashValue2.SequenceEqual(actualhash))
                {
                    Console.WriteLine("Active DIFI 1 - Save 2.");
                    savindex = 1;
                }
                else
                {
                    Console.WriteLine("ERROR: NO ACTIVE DIFI HASH MATCH");
                    savindex = 2;
                }
            }
            if ((data[0x168] ^ 1) != savindex && savindex != 2)
            {
                Console.WriteLine("ERROR: ACTIVE BLOCK MISMATCH");
                savindex = 2;
            }
            return savindex;
        }
        internal static ushort ccitt16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x8000) > 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }
    }
}
