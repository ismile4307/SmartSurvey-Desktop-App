######################################################################

Project Name : Eagle (April 26)
Project Code : 261701
Script Version : 1.0.0.7
Script Name : KNI21858
Scripted By : Md Habibur Rahman
Date : 04.05.2026

######################################################################

*QUESTION FIFSInfo *FIFS
Information Of FI and FS

#############################Start Main Script########################

*QUESTION Month *SR 
Please Select Month
4:April'26
#5:May'26
#6:June'26

#*INCLUDE Month [4] Bhaiya eita korlei to hoy

*QUESTION LanOfIntv *SR *DUMMY2 
Language of Interview
1:English
2:Bengali
3:Hindi
4:Kannada
5:Tamil
6:Telugu
7:Gujrati

*INCLUDE LanOfIntv LanguageOf[Interview]

#*QUESTION MAreaBan *SR *DUMMY2
#June-Wilsongarden September-Chamarajapet December-Vasnthnagar March-Shanthinagar
#1:Wilsongarden
#2:Chamarajapet
#3:Vasnthnagar
#4:Shanthinagar
#
#*IF [Month=12] *INCLUDE MAreaBan [4]

*QUESTION DummyCentre *MR *DUMMY2
Q1. Center
1:Mumbai
2:Delhi
3:Chennai
4:Bangalore
5:Kolkata
6:Hyderabad
7:Ahmedabad
8:Pune
9:Lucknow
10:Jaipur
11:Patna
12:Indore
99:NONE OF THE ABOVE

*IF [UserIdOf[Interview]=StringOf[eagle04261]] *INCLUDE DummyCentre [3;5;99]
*IF [UserIdOf[Interview]=StringOf[eagle04262]] *INCLUDE DummyCentre [1;2;4;6;7;8;9;10;11;12;99]

# 
*QUESTION Centre *SR *INCLUDE [DummyCentre]
Q1. Center (<font color="#F5AFA3">CODE APPROPRIATELY</font>) [SA]
1:Mumbai
2:Delhi
3:Chennai
4:Bangalore
5:Kolkata
6:Hyderabad
7:Ahmedabad
8:Pune
9:Lucknow
10:Jaipur
11:Patna
12:Indore
99:NONE OF THE ABOVE

*IF [Centre=99] *GOTO TN

*QUESTION HD1 *SR *DUMMY2
HD1. Market auto code (Hidden in the Live Link)
1:Top 4 metros
2:Next 4 Metros
3:Non metros

*IF [Centre=1 | Centre=2 | Centre=3 | Centre=4] *INCLUDE HD1 [1]
*IF [Centre=5 | Centre=6 | Centre=7 | Centre=8] *INCLUDE HD1 [2]
*IF [Centre=9 | Centre=10 | Centre=11 | Centre=12] *INCLUDE HD1 [3]

*LIST "ZoneList"
1:East
2:West
3:North
4:South
5:Central

*QUESTION DummyZone *MR *DUMMY2
Dummy Zone Name
*USELIST "ZoneList"

*IF [Centre=1 | Centre=2 | Centre=3 | Centre=4 | Centre=5 | Centre=6 | Centre=7 | Centre=8] *INCLUDE DummyZone [1;2;3;4;5]
*IF [Centre=9 | Centre=10 | Centre=11 | Centre=12] *INCLUDE DummyZone [1;2;3;4]

*QUESTION Zone *SR *INCLUDE [DummyZone]
Select Zone Name
*USELIST "ZoneList"

*LIST "AreaList"
1:Mulund 
2:Vikhroli 
3:Ghatkopar
4:Kanjurmarg
5:Bhandup 
6:Kurla 
7:Bandra
8:Mahim 
9:Khar
10:Santacruz 
11:Andheri 
12:Vileparle
13:Boriwali 
14:Kandiwali
15:Malad 
16:Jogeshwari 
17:Dahisar
18:Goregaon 
19:Dadar 
20:Parel 
21:Byculla
22:Mahalaxmi
23:Lower Parel
24:Charni Road
25:Mahul Road 
26:Sion 
27:G.T.B Nagar
28:Chembur 
29:Wadala
30:Matunga
31:Laxmi nagar
32:Geeta colony
33:Sakar pur
34:Mayur vihar
35:Pandav nagar 
36:Ashok nagar
37:Janak Puri
38:Uttam Nagar
39:Vikaspuri
40:Najafgarh
41:Subhash nagar
42:Hari nagar
43:Pitam Pura
44:Azadpur
45:Rohini
46:Keshav puram
47:Ashok vihar
48:Kanaiya nagar
49:Govind Puri
50:Kalika Ji
51:Nehru nagar1
52:Lajpat Nagar
53:Badarpur
54:Saket
55:Pahar ganj
56:Dariya ganj
57:Karol Bagh
58:Minto road
59:Patel Nagar
60:Anand parvat
61:Triplcane
62:Velachery
63:Mylapore
64:Mandaveli
65:Adyar
66:Guindy
67:Arumbakkam
68:Mugappair 
69:Annanagar
70:MMDA
71:Aminjikarai
72:Choolaimedu
73:Perambur
74:villivakkam
75:ayanavaram
76:padi
77:old washernpet 
78:tondairpet 
79:T.Nagar
80:Westmambalam
81:CIT nagar
82:Saidapet
83:Nungambakkam
84:Kodambakkam
85:Purasaiwakkam
86:Ambatur
87:Choolai
88:Avadi
89:Thirumullaivoyal
90:Ayapakam
91:Indranagar
92:Koramagala
93:Sarvagna Nagar
94:BTM Layout
95:New Thipasandra
96:Banasawadi
97:Vijayanagar
98:Nagarabhavi
99:Mahalakshmilayout
100:Basaveshwaranagar
101:Rajajinagar
102:Sunkadakatte
103:Cox Town
104:Yeshwanthpur
105:Hebbala
106:Sanjaynagar
107:Malleshwaram
108:Yelahanka
109:Banashankari
110:Jayanagar
111:Basavanagudi
112:Rajarajeshwarinagar
113:Srinagar
114:J P Nagar
115:Willsongarden
116:Shanthinagar
117:Chamarajapet
118:Ulsoor
119:Vasnthanagar
120:Wilsongarden, Chamarajapet, Vasnthnagar, Shanthinagar
121:Beleghata CIT
122:Ultadanga
123:Phoolbagan
124:Sarkar Bazar
125:Santoshpur
126:Kasba
127:Chowrasta
128:Behala Thana
129:Pathakpara
130:New Alipore
131:Tollygunge
132:Khiddirpur
133:Paikpara
134:Bagbazar-Cossipore
135:Belgachia
136:Shyambazar
137:Sovabazar
138:Maniktala
139:Dhakuria
140:Yadavpur
141:Baghajatin
142:Azadgarh
143:Garia
144:Prince Anwar Shah
145:Sealdah
146:Ananda Palit
147:Entally
148:Picnic Garden
149:Kalighat
150:Haltu
151:Dilsukhnagar
152:Kothapet
153:LB nagar
154:Uppal
155:Nagole
156:Ramanthapur
157:Panjagutta
158:Khairatabad
159:Ameerpet
160:Somajiguda
161:Yousfguda
162:SR Nagar
163:Secunderabad
164:Warasiguda
165:Seethafalmandi
166:Padmarao Nagar
167:Kawadiguda
168:West Marrepally
169:Santhoshnagar
170:Karmanghat
171:Saidabad
172:Saroornagar
173:Chanchalguda
174:Maddanapet
175:Ramnagar
176:Vidyanagar
177:Amberpet
178:Kachiguda
179:Narayanguda
180:Himayatnagar
181:Bapunagar
182:Odhav
183:Nikol
184:Saraspur
185:Vastral
186:Amraiwadi
187:Ranip
188:Chanakyapuri
189:Nirnaynagar
190:Gota
191:Vejalpur
192:Sabarmati
193:Krushnanagar
194:Naroda
195:Saijpurbhoga
196:Nava Vadaj
197:Chandkheda
198:Shahibagh
199:Ghodasar
200:Isanpur
201:Maninagar
202:Vatwa
203:Paldi
204:Vasna
205:Khadia
206:Dariyapur
207:Rakhial
208:Kalupur
209:Behrampura
210:Gomtipur
211:HADPASAR
212:Kondhwa
213:Mundhwa
214:Chandan Nagar
215:KALEPADAL
216:Camp
217:Pashan 
218:Hingne
219:Kothrud
220:Earandwane
221:WARJE
222:KARVE NAGAR
223:Shivaji Nagar
224:GHOKHALE NAGAR
225:Vishrant Wadi
226:Aundh
227:Bopodi
228:Sangam Wadi
229:DHANKAWADI
230:KATRAJ
231:DATTAWADI
232:Parvati 
233:Balaji Nagar
234:Bibwewadi
235:BHAWANI PETH
236:NANA PETH
237:GURUWAR PETH
238:Narayan Peth 
239:Sadashiv Peth
240:KASBA PETH
241:GOMTI NAGAR
242:MUNSHI PULIYA
243:NISHAT GANJ
244:INDIRA NAGAR
245:MAHANAGAR
246:NARHI
247:DALIGANJ
248:AZAD NAGAR
249:CHOUK
250:RASTOGI NAGAR
251:KHADRA
252:THAKUR GANJ
253:TRIVENY NAGAR
254:ALIGANJ
255:BHARAT NAGAR
256:JANKIPURAM
257:VIKAS NAGAR
258:FAIZULLA GANJ
259:RAJAJIPURAM
260:ALAMNAGAR
261:ALAMBAGH
262:LAL KUWAN
263:HUSAIN GANJ
264:LDA COLONY
265:Shastri Nagar1
266:Nehru Nagar2
267:Vidhydhar Nagar
268:Khatipura
269:Murlipura
270:Alkapuri
271:Shanti Nagar-1
272:Nandpuri
273:Kartarpura
274:Hatroi
275:Sanjay Nagar-2
276:Vaishali
277:Mansrover
278:Gurjer Ki Thadi
279:Shyam Nagar-2nd
280:Hassanpura
281:Ashokpura-B block
282:Lal Kothi-Ablock
283:Malviya Nagar1
284:Pratap Nagar-1
285:Sanganer Railway clony
286:Jhalana doongri
287:BARKAT NAGAR 
288:Raja Park
289:DEEP NAGAR 
290:PASSCHIM DARWAZA
291:GAI GHAT 
292:NOON KA CHOURAHA
293:TRIPOLIA 
294:KUMHRAR
295:MANDIRI
296:PUNAI CHAK 
297:ANISABAD
298:SHASTRI NAGAR2
299:BEUR
300:SARISTABAD 
301:SULTAN GANJ 
302:MAHENDRU 
303:SAIDPUR
304:BAKARGUNJ
305:KURJI 
306:MAINPURA 
307:SIPARA 
308:RAM KRISHNA NAGAR
309:POSTAL PARK
310:KANKARBAG 
311:HANUMAN NGAR 
312:YOGIPUR
313:Sudama Nagar
314:Dwarkapuri
315:Scheme No.71
316:Janta Colony
317:Rajmohalla
318:Vishwakarma Nagar
319:Pardeshipura
320:Sukhliya
321:Nanda Nagar
322:Ambedkar Nagar
323:Saket nagar
324:Palasia
325:Vijay Nagar
326:Malviya Nagar2
327:Mahalaxmi Nagar
328:Scheme No.54
329:Scheme No.78
330:Sheetal Nagar
331:Kalani Nagar
332:Nagin Nagar
333:Palhar Nagar
334:Scheme no.51
335:Kushwah Nagar
336:Banganga

*QUESTION DummyArea *MR *DUMMY2
Dummy Area Name
*USELIST "AreaList"

*IF [Centre=1 & Zone=1] *INCLUDE DummyArea [1]
*IF [Centre=1 & Zone=1] *INCLUDE DummyArea [2]
*IF [Centre=1 & Zone=1] *INCLUDE DummyArea [3]
*IF [Centre=1 & Zone=1] *INCLUDE DummyArea [4]
*IF [Centre=1 & Zone=1] *INCLUDE DummyArea [5]
*IF [Centre=1 & Zone=1] *INCLUDE DummyArea [6]
*IF [Centre=1 & Zone=2] *INCLUDE DummyArea [7]
*IF [Centre=1 & Zone=2] *INCLUDE DummyArea [8]
*IF [Centre=1 & Zone=2] *INCLUDE DummyArea [9]
*IF [Centre=1 & Zone=2] *INCLUDE DummyArea [10]
*IF [Centre=1 & Zone=2] *INCLUDE DummyArea [11]
*IF [Centre=1 & Zone=2] *INCLUDE DummyArea [12]
*IF [Centre=1 & Zone=3] *INCLUDE DummyArea [13]
*IF [Centre=1 & Zone=3] *INCLUDE DummyArea [14]
*IF [Centre=1 & Zone=3] *INCLUDE DummyArea [15]
*IF [Centre=1 & Zone=3] *INCLUDE DummyArea [16]
*IF [Centre=1 & Zone=3] *INCLUDE DummyArea [17]
*IF [Centre=1 & Zone=3] *INCLUDE DummyArea [18]
*IF [Centre=1 & Zone=4] *INCLUDE DummyArea [19]
*IF [Centre=1 & Zone=4] *INCLUDE DummyArea [20]
*IF [Centre=1 & Zone=4] *INCLUDE DummyArea [21]
*IF [Centre=1 & Zone=4] *INCLUDE DummyArea [22]
*IF [Centre=1 & Zone=4] *INCLUDE DummyArea [23]
*IF [Centre=1 & Zone=4] *INCLUDE DummyArea [24]
*IF [Centre=1 & Zone=5] *INCLUDE DummyArea [25]
*IF [Centre=1 & Zone=5] *INCLUDE DummyArea [26]
*IF [Centre=1 & Zone=5] *INCLUDE DummyArea [27]
*IF [Centre=1 & Zone=5] *INCLUDE DummyArea [28]
*IF [Centre=1 & Zone=5] *INCLUDE DummyArea [29]
*IF [Centre=1 & Zone=5] *INCLUDE DummyArea [30]
*IF [Centre=2 & Zone=1] *INCLUDE DummyArea [31]
*IF [Centre=2 & Zone=1] *INCLUDE DummyArea [32]
*IF [Centre=2 & Zone=1] *INCLUDE DummyArea [33]
*IF [Centre=2 & Zone=1] *INCLUDE DummyArea [34]
*IF [Centre=2 & Zone=1] *INCLUDE DummyArea [35]
*IF [Centre=2 & Zone=1] *INCLUDE DummyArea [36]
*IF [Centre=2 & Zone=2] *INCLUDE DummyArea [37]
*IF [Centre=2 & Zone=2] *INCLUDE DummyArea [38]
*IF [Centre=2 & Zone=2] *INCLUDE DummyArea [39]
*IF [Centre=2 & Zone=2] *INCLUDE DummyArea [40]
*IF [Centre=2 & Zone=2] *INCLUDE DummyArea [41]
*IF [Centre=2 & Zone=2] *INCLUDE DummyArea [42]
*IF [Centre=2 & Zone=3] *INCLUDE DummyArea [43]
*IF [Centre=2 & Zone=3] *INCLUDE DummyArea [44]
*IF [Centre=2 & Zone=3] *INCLUDE DummyArea [45]
*IF [Centre=2 & Zone=3] *INCLUDE DummyArea [46]
*IF [Centre=2 & Zone=3] *INCLUDE DummyArea [47]
*IF [Centre=2 & Zone=3] *INCLUDE DummyArea [48]
*IF [Centre=2 & Zone=4] *INCLUDE DummyArea [49]
*IF [Centre=2 & Zone=4] *INCLUDE DummyArea [50]
*IF [Centre=2 & Zone=4] *INCLUDE DummyArea [51]
*IF [Centre=2 & Zone=4] *INCLUDE DummyArea [52]
*IF [Centre=2 & Zone=4] *INCLUDE DummyArea [53]
*IF [Centre=2 & Zone=4] *INCLUDE DummyArea [54]
*IF [Centre=2 & Zone=5] *INCLUDE DummyArea [55]
*IF [Centre=2 & Zone=5] *INCLUDE DummyArea [56]
*IF [Centre=2 & Zone=5] *INCLUDE DummyArea [57]
*IF [Centre=2 & Zone=5] *INCLUDE DummyArea [58]
*IF [Centre=2 & Zone=5] *INCLUDE DummyArea [59]
*IF [Centre=2 & Zone=5] *INCLUDE DummyArea [60]
*IF [Centre=3 & Zone=1] *INCLUDE DummyArea [61]
*IF [Centre=3 & Zone=1] *INCLUDE DummyArea [62]
*IF [Centre=3 & Zone=1] *INCLUDE DummyArea [63]
*IF [Centre=3 & Zone=1] *INCLUDE DummyArea [64]
*IF [Centre=3 & Zone=1] *INCLUDE DummyArea [65]
*IF [Centre=3 & Zone=1] *INCLUDE DummyArea [66]
*IF [Centre=3 & Zone=2] *INCLUDE DummyArea [67]
*IF [Centre=3 & Zone=2] *INCLUDE DummyArea [68]
*IF [Centre=3 & Zone=2] *INCLUDE DummyArea [69]
*IF [Centre=3 & Zone=2] *INCLUDE DummyArea [70]
*IF [Centre=3 & Zone=2] *INCLUDE DummyArea [71]
*IF [Centre=3 & Zone=2] *INCLUDE DummyArea [72]
*IF [Centre=3 & Zone=3] *INCLUDE DummyArea [73]
*IF [Centre=3 & Zone=3] *INCLUDE DummyArea [74]
*IF [Centre=3 & Zone=3] *INCLUDE DummyArea [75]
*IF [Centre=3 & Zone=3] *INCLUDE DummyArea [76]
*IF [Centre=3 & Zone=3] *INCLUDE DummyArea [77]
*IF [Centre=3 & Zone=3] *INCLUDE DummyArea [78]
*IF [Centre=3 & Zone=4] *INCLUDE DummyArea [79]
*IF [Centre=3 & Zone=4] *INCLUDE DummyArea [80]
*IF [Centre=3 & Zone=4] *INCLUDE DummyArea [81]
*IF [Centre=3 & Zone=4] *INCLUDE DummyArea [82]
*IF [Centre=3 & Zone=4] *INCLUDE DummyArea [83]
*IF [Centre=3 & Zone=4] *INCLUDE DummyArea [84]
*IF [Centre=3 & Zone=5] *INCLUDE DummyArea [85]
*IF [Centre=3 & Zone=5] *INCLUDE DummyArea [86]
*IF [Centre=3 & Zone=5] *INCLUDE DummyArea [87]
*IF [Centre=3 & Zone=5] *INCLUDE DummyArea [88]
*IF [Centre=3 & Zone=5] *INCLUDE DummyArea [89]
*IF [Centre=3 & Zone=5] *INCLUDE DummyArea [90]
*IF [Centre=4 & Zone=1] *INCLUDE DummyArea [91]
*IF [Centre=4 & Zone=1] *INCLUDE DummyArea [92]
*IF [Centre=4 & Zone=1] *INCLUDE DummyArea [93]
*IF [Centre=4 & Zone=1] *INCLUDE DummyArea [94]
*IF [Centre=4 & Zone=1] *INCLUDE DummyArea [95]
*IF [Centre=4 & Zone=1] *INCLUDE DummyArea [96]
*IF [Centre=4 & Zone=2] *INCLUDE DummyArea [97]
*IF [Centre=4 & Zone=2] *INCLUDE DummyArea [98]
*IF [Centre=4 & Zone=2] *INCLUDE DummyArea [99]
*IF [Centre=4 & Zone=2] *INCLUDE DummyArea [100]
*IF [Centre=4 & Zone=2] *INCLUDE DummyArea [101]
*IF [Centre=4 & Zone=2] *INCLUDE DummyArea [102]
*IF [Centre=4 & Zone=3] *INCLUDE DummyArea [103]
*IF [Centre=4 & Zone=3] *INCLUDE DummyArea [104]
*IF [Centre=4 & Zone=3] *INCLUDE DummyArea [105]
*IF [Centre=4 & Zone=3] *INCLUDE DummyArea [106]
*IF [Centre=4 & Zone=3] *INCLUDE DummyArea [107]
*IF [Centre=4 & Zone=3] *INCLUDE DummyArea [108]
*IF [Centre=4 & Zone=4] *INCLUDE DummyArea [109]
*IF [Centre=4 & Zone=4] *INCLUDE DummyArea [110]
*IF [Centre=4 & Zone=4] *INCLUDE DummyArea [111]
*IF [Centre=4 & Zone=4] *INCLUDE DummyArea [112]
*IF [Centre=4 & Zone=4] *INCLUDE DummyArea [113]
*IF [Centre=4 & Zone=4] *INCLUDE DummyArea [114]
*IF [Centre=4 & Zone=5] *INCLUDE DummyArea [115]
*IF [Centre=4 & Zone=5] *INCLUDE DummyArea [116]
*IF [Centre=4 & Zone=5] *INCLUDE DummyArea [117]
*IF [Centre=4 & Zone=5] *INCLUDE DummyArea [118]
*IF [Centre=4 & Zone=5] *INCLUDE DummyArea [119]
*IF [Centre=4 & Zone=5] *INCLUDE DummyArea [120]
*IF [Centre=5 & Zone=1] *INCLUDE DummyArea [121]
*IF [Centre=5 & Zone=1] *INCLUDE DummyArea [122]
*IF [Centre=5 & Zone=1] *INCLUDE DummyArea [123]
*IF [Centre=5 & Zone=1] *INCLUDE DummyArea [124]
*IF [Centre=5 & Zone=1] *INCLUDE DummyArea [125]
*IF [Centre=5 & Zone=1] *INCLUDE DummyArea [126]
*IF [Centre=5 & Zone=2] *INCLUDE DummyArea [127]
*IF [Centre=5 & Zone=2] *INCLUDE DummyArea [128]
*IF [Centre=5 & Zone=2] *INCLUDE DummyArea [129]
*IF [Centre=5 & Zone=2] *INCLUDE DummyArea [130]
*IF [Centre=5 & Zone=2] *INCLUDE DummyArea [131]
*IF [Centre=5 & Zone=2] *INCLUDE DummyArea [132]
*IF [Centre=5 & Zone=3] *INCLUDE DummyArea [133]
*IF [Centre=5 & Zone=3] *INCLUDE DummyArea [134]
*IF [Centre=5 & Zone=3] *INCLUDE DummyArea [135]
*IF [Centre=5 & Zone=3] *INCLUDE DummyArea [136]
*IF [Centre=5 & Zone=3] *INCLUDE DummyArea [137]
*IF [Centre=5 & Zone=3] *INCLUDE DummyArea [138]
*IF [Centre=5 & Zone=4] *INCLUDE DummyArea [139]
*IF [Centre=5 & Zone=4] *INCLUDE DummyArea [140]
*IF [Centre=5 & Zone=4] *INCLUDE DummyArea [141]
*IF [Centre=5 & Zone=4] *INCLUDE DummyArea [142]
*IF [Centre=5 & Zone=4] *INCLUDE DummyArea [143]
*IF [Centre=5 & Zone=4] *INCLUDE DummyArea [144]
*IF [Centre=5 & Zone=5] *INCLUDE DummyArea [145]
*IF [Centre=5 & Zone=5] *INCLUDE DummyArea [146]
*IF [Centre=5 & Zone=5] *INCLUDE DummyArea [147]
*IF [Centre=5 & Zone=5] *INCLUDE DummyArea [148]
*IF [Centre=5 & Zone=5] *INCLUDE DummyArea [149]
*IF [Centre=5 & Zone=5] *INCLUDE DummyArea [150]
*IF [Centre=6 & Zone=1] *INCLUDE DummyArea [151]
*IF [Centre=6 & Zone=1] *INCLUDE DummyArea [152]
*IF [Centre=6 & Zone=1] *INCLUDE DummyArea [153]
*IF [Centre=6 & Zone=1] *INCLUDE DummyArea [154]
*IF [Centre=6 & Zone=1] *INCLUDE DummyArea [155]
*IF [Centre=6 & Zone=1] *INCLUDE DummyArea [156]
*IF [Centre=6 & Zone=2] *INCLUDE DummyArea [157]
*IF [Centre=6 & Zone=2] *INCLUDE DummyArea [158]
*IF [Centre=6 & Zone=2] *INCLUDE DummyArea [159]
*IF [Centre=6 & Zone=2] *INCLUDE DummyArea [160]
*IF [Centre=6 & Zone=2] *INCLUDE DummyArea [161]
*IF [Centre=6 & Zone=2] *INCLUDE DummyArea [162]
*IF [Centre=6 & Zone=3] *INCLUDE DummyArea [163]
*IF [Centre=6 & Zone=3] *INCLUDE DummyArea [164]
*IF [Centre=6 & Zone=3] *INCLUDE DummyArea [165]
*IF [Centre=6 & Zone=3] *INCLUDE DummyArea [166]
*IF [Centre=6 & Zone=3] *INCLUDE DummyArea [167]
*IF [Centre=6 & Zone=3] *INCLUDE DummyArea [168]
*IF [Centre=6 & Zone=4] *INCLUDE DummyArea [169]
*IF [Centre=6 & Zone=4] *INCLUDE DummyArea [170]
*IF [Centre=6 & Zone=4] *INCLUDE DummyArea [171]
*IF [Centre=6 & Zone=4] *INCLUDE DummyArea [172]
*IF [Centre=6 & Zone=4] *INCLUDE DummyArea [173]
*IF [Centre=6 & Zone=4] *INCLUDE DummyArea [174]
*IF [Centre=6 & Zone=5] *INCLUDE DummyArea [175]
*IF [Centre=6 & Zone=5] *INCLUDE DummyArea [176]
*IF [Centre=6 & Zone=5] *INCLUDE DummyArea [177]
*IF [Centre=6 & Zone=5] *INCLUDE DummyArea [178]
*IF [Centre=6 & Zone=5] *INCLUDE DummyArea [179]
*IF [Centre=6 & Zone=5] *INCLUDE DummyArea [180]
*IF [Centre=7 & Zone=1] *INCLUDE DummyArea [181]
*IF [Centre=7 & Zone=1] *INCLUDE DummyArea [182]
*IF [Centre=7 & Zone=1] *INCLUDE DummyArea [183]
*IF [Centre=7 & Zone=1] *INCLUDE DummyArea [184]
*IF [Centre=7 & Zone=1] *INCLUDE DummyArea [185]
*IF [Centre=7 & Zone=1] *INCLUDE DummyArea [186]
*IF [Centre=7 & Zone=2] *INCLUDE DummyArea [187]
*IF [Centre=7 & Zone=2] *INCLUDE DummyArea [188]
*IF [Centre=7 & Zone=2] *INCLUDE DummyArea [189]
*IF [Centre=7 & Zone=2] *INCLUDE DummyArea [190]
*IF [Centre=7 & Zone=2] *INCLUDE DummyArea [191]
*IF [Centre=7 & Zone=2] *INCLUDE DummyArea [192]
*IF [Centre=7 & Zone=3] *INCLUDE DummyArea [193]
*IF [Centre=7 & Zone=3] *INCLUDE DummyArea [194]
*IF [Centre=7 & Zone=3] *INCLUDE DummyArea [195]
*IF [Centre=7 & Zone=3] *INCLUDE DummyArea [196]
*IF [Centre=7 & Zone=3] *INCLUDE DummyArea [197]
*IF [Centre=7 & Zone=3] *INCLUDE DummyArea [198]
*IF [Centre=7 & Zone=4] *INCLUDE DummyArea [199]
*IF [Centre=7 & Zone=4] *INCLUDE DummyArea [200]
*IF [Centre=7 & Zone=4] *INCLUDE DummyArea [201]
*IF [Centre=7 & Zone=4] *INCLUDE DummyArea [202]
*IF [Centre=7 & Zone=4] *INCLUDE DummyArea [203]
*IF [Centre=7 & Zone=4] *INCLUDE DummyArea [204]
*IF [Centre=7 & Zone=5] *INCLUDE DummyArea [205]
*IF [Centre=7 & Zone=5] *INCLUDE DummyArea [206]
*IF [Centre=7 & Zone=5] *INCLUDE DummyArea [207]
*IF [Centre=7 & Zone=5] *INCLUDE DummyArea [208]
*IF [Centre=7 & Zone=5] *INCLUDE DummyArea [209]
*IF [Centre=7 & Zone=5] *INCLUDE DummyArea [210]
*IF [Centre=8 & Zone=1] *INCLUDE DummyArea [211]
*IF [Centre=8 & Zone=1] *INCLUDE DummyArea [212]
*IF [Centre=8 & Zone=1] *INCLUDE DummyArea [213]
*IF [Centre=8 & Zone=1] *INCLUDE DummyArea [214]
*IF [Centre=8 & Zone=1] *INCLUDE DummyArea [215]
*IF [Centre=8 & Zone=1] *INCLUDE DummyArea [216]
*IF [Centre=8 & Zone=2] *INCLUDE DummyArea [217]
*IF [Centre=8 & Zone=2] *INCLUDE DummyArea [218]
*IF [Centre=8 & Zone=2] *INCLUDE DummyArea [219]
*IF [Centre=8 & Zone=2] *INCLUDE DummyArea [220]
*IF [Centre=8 & Zone=2] *INCLUDE DummyArea [221]
*IF [Centre=8 & Zone=2] *INCLUDE DummyArea [222]
*IF [Centre=8 & Zone=3] *INCLUDE DummyArea [223]
*IF [Centre=8 & Zone=3] *INCLUDE DummyArea [224]
*IF [Centre=8 & Zone=3] *INCLUDE DummyArea [225]
*IF [Centre=8 & Zone=3] *INCLUDE DummyArea [226]
*IF [Centre=8 & Zone=3] *INCLUDE DummyArea [227]
*IF [Centre=8 & Zone=3] *INCLUDE DummyArea [228]
*IF [Centre=8 & Zone=4] *INCLUDE DummyArea [229]
*IF [Centre=8 & Zone=4] *INCLUDE DummyArea [230]
*IF [Centre=8 & Zone=4] *INCLUDE DummyArea [231]
*IF [Centre=8 & Zone=4] *INCLUDE DummyArea [232]
*IF [Centre=8 & Zone=4] *INCLUDE DummyArea [233]
*IF [Centre=8 & Zone=4] *INCLUDE DummyArea [234]
*IF [Centre=8 & Zone=5] *INCLUDE DummyArea [235]
*IF [Centre=8 & Zone=5] *INCLUDE DummyArea [236]
*IF [Centre=8 & Zone=5] *INCLUDE DummyArea [237]
*IF [Centre=8 & Zone=5] *INCLUDE DummyArea [238]
*IF [Centre=8 & Zone=5] *INCLUDE DummyArea [239]
*IF [Centre=8 & Zone=5] *INCLUDE DummyArea [240]
*IF [Centre=9 & Zone=1] *INCLUDE DummyArea [241]
*IF [Centre=9 & Zone=1] *INCLUDE DummyArea [242]
*IF [Centre=9 & Zone=1] *INCLUDE DummyArea [243]
*IF [Centre=9 & Zone=1] *INCLUDE DummyArea [244]
*IF [Centre=9 & Zone=1] *INCLUDE DummyArea [245]
*IF [Centre=9 & Zone=1] *INCLUDE DummyArea [246]
*IF [Centre=9 & Zone=2] *INCLUDE DummyArea [247]
*IF [Centre=9 & Zone=2] *INCLUDE DummyArea [248]
*IF [Centre=9 & Zone=2] *INCLUDE DummyArea [249]
*IF [Centre=9 & Zone=2] *INCLUDE DummyArea [250]
*IF [Centre=9 & Zone=2] *INCLUDE DummyArea [251]
*IF [Centre=9 & Zone=2] *INCLUDE DummyArea [252]
*IF [Centre=9 & Zone=3] *INCLUDE DummyArea [253]
*IF [Centre=9 & Zone=3] *INCLUDE DummyArea [254]
*IF [Centre=9 & Zone=3] *INCLUDE DummyArea [255]
*IF [Centre=9 & Zone=3] *INCLUDE DummyArea [256]
*IF [Centre=9 & Zone=3] *INCLUDE DummyArea [257]
*IF [Centre=9 & Zone=3] *INCLUDE DummyArea [258]
*IF [Centre=9 & Zone=4] *INCLUDE DummyArea [259]
*IF [Centre=9 & Zone=4] *INCLUDE DummyArea [260]
*IF [Centre=9 & Zone=4] *INCLUDE DummyArea [261]
*IF [Centre=9 & Zone=4] *INCLUDE DummyArea [262]
*IF [Centre=9 & Zone=4] *INCLUDE DummyArea [263]
*IF [Centre=9 & Zone=4] *INCLUDE DummyArea [264]
*IF [Centre=10 & Zone=1] *INCLUDE DummyArea [265]
*IF [Centre=10 & Zone=1] *INCLUDE DummyArea [266]
*IF [Centre=10 & Zone=1] *INCLUDE DummyArea [267]
*IF [Centre=10 & Zone=1] *INCLUDE DummyArea [268]
*IF [Centre=10 & Zone=1] *INCLUDE DummyArea [269]
*IF [Centre=10 & Zone=1] *INCLUDE DummyArea [270]
*IF [Centre=10 & Zone=2] *INCLUDE DummyArea [271]
*IF [Centre=10 & Zone=2] *INCLUDE DummyArea [272]
*IF [Centre=10 & Zone=2] *INCLUDE DummyArea [273]
*IF [Centre=10 & Zone=2] *INCLUDE DummyArea [274]
*IF [Centre=10 & Zone=2] *INCLUDE DummyArea [275]
*IF [Centre=10 & Zone=2] *INCLUDE DummyArea [276]
*IF [Centre=10 & Zone=3] *INCLUDE DummyArea [277]
*IF [Centre=10 & Zone=3] *INCLUDE DummyArea [278]
*IF [Centre=10 & Zone=3] *INCLUDE DummyArea [279]
*IF [Centre=10 & Zone=3] *INCLUDE DummyArea [280]
*IF [Centre=10 & Zone=3] *INCLUDE DummyArea [281]
*IF [Centre=10 & Zone=3] *INCLUDE DummyArea [282]
*IF [Centre=10 & Zone=4] *INCLUDE DummyArea [283]
*IF [Centre=10 & Zone=4] *INCLUDE DummyArea [284]
*IF [Centre=10 & Zone=4] *INCLUDE DummyArea [285]
*IF [Centre=10 & Zone=4] *INCLUDE DummyArea [286]
*IF [Centre=10 & Zone=4] *INCLUDE DummyArea [287]
*IF [Centre=10 & Zone=4] *INCLUDE DummyArea [288]
*IF [Centre=11 & Zone=1] *INCLUDE DummyArea [289]
*IF [Centre=11 & Zone=1] *INCLUDE DummyArea [290]
*IF [Centre=11 & Zone=1] *INCLUDE DummyArea [291]
*IF [Centre=11 & Zone=1] *INCLUDE DummyArea [292]
*IF [Centre=11 & Zone=1] *INCLUDE DummyArea [293]
*IF [Centre=11 & Zone=1] *INCLUDE DummyArea [294]
*IF [Centre=11 & Zone=2] *INCLUDE DummyArea [295]
*IF [Centre=11 & Zone=2] *INCLUDE DummyArea [296]
*IF [Centre=11 & Zone=2] *INCLUDE DummyArea [297]
*IF [Centre=11 & Zone=2] *INCLUDE DummyArea [298]
*IF [Centre=11 & Zone=2] *INCLUDE DummyArea [299]
*IF [Centre=11 & Zone=2] *INCLUDE DummyArea [300]
*IF [Centre=11 & Zone=3] *INCLUDE DummyArea [301]
*IF [Centre=11 & Zone=3] *INCLUDE DummyArea [302]
*IF [Centre=11 & Zone=3] *INCLUDE DummyArea [303]
*IF [Centre=11 & Zone=3] *INCLUDE DummyArea [304]
*IF [Centre=11 & Zone=3] *INCLUDE DummyArea [305]
*IF [Centre=11 & Zone=3] *INCLUDE DummyArea [306]
*IF [Centre=11 & Zone=4] *INCLUDE DummyArea [307]
*IF [Centre=11 & Zone=4] *INCLUDE DummyArea [308]
*IF [Centre=11 & Zone=4] *INCLUDE DummyArea [309]
*IF [Centre=11 & Zone=4] *INCLUDE DummyArea [310]
*IF [Centre=11 & Zone=4] *INCLUDE DummyArea [311]
*IF [Centre=11 & Zone=4] *INCLUDE DummyArea [312]
*IF [Centre=12 & Zone=1] *INCLUDE DummyArea [313]
*IF [Centre=12 & Zone=1] *INCLUDE DummyArea [314]
*IF [Centre=12 & Zone=1] *INCLUDE DummyArea [315]
*IF [Centre=12 & Zone=1] *INCLUDE DummyArea [316]
*IF [Centre=12 & Zone=1] *INCLUDE DummyArea [317]
*IF [Centre=12 & Zone=1] *INCLUDE DummyArea [318]
*IF [Centre=12 & Zone=2] *INCLUDE DummyArea [319]
*IF [Centre=12 & Zone=2] *INCLUDE DummyArea [320]
*IF [Centre=12 & Zone=2] *INCLUDE DummyArea [321]
*IF [Centre=12 & Zone=2] *INCLUDE DummyArea [322]
*IF [Centre=12 & Zone=2] *INCLUDE DummyArea [323]
*IF [Centre=12 & Zone=2] *INCLUDE DummyArea [324]
*IF [Centre=12 & Zone=3] *INCLUDE DummyArea [325]
*IF [Centre=12 & Zone=3] *INCLUDE DummyArea [326]
*IF [Centre=12 & Zone=3] *INCLUDE DummyArea [327]
*IF [Centre=12 & Zone=3] *INCLUDE DummyArea [328]
*IF [Centre=12 & Zone=3] *INCLUDE DummyArea [329]
*IF [Centre=12 & Zone=3] *INCLUDE DummyArea [330]
*IF [Centre=12 & Zone=4] *INCLUDE DummyArea [331]
*IF [Centre=12 & Zone=4] *INCLUDE DummyArea [332]
*IF [Centre=12 & Zone=4] *INCLUDE DummyArea [333]
*IF [Centre=12 & Zone=4] *INCLUDE DummyArea [334]
*IF [Centre=12 & Zone=4] *INCLUDE DummyArea [335]
*IF [Centre=12 & Zone=4] *INCLUDE DummyArea [336]

*QUESTION Area *SR *INCLUDE [DummyArea] *COLUMN 2
Select Area
*USELIST "AreaList"

*QUESTION SubArea *OPEN
Please write down the Sub Area Name

*QUESTION RespType *SR
Select Type of Interview
1:Normal market hunting
2:Referral/ reference based
3:Pre-appointment based

*LIST "YN"
1:Yes
2:No

*QUESTION Intro *SR 
[SPEAK TO AN ADULT MEMBER OF THE HOUSE]
<br>Hello!
My name is ………
Thank you for agreeing to this survey, we appreciate your time and value your answers. This survey is on the behalf of decrypt a Market Research Company. We hope that you would be willing to answer a few questions for us today. This survey should not take more than 30 mins of your time.
*USELIST "YN"

*IF [Intro=2] *GOTO TN

*QUESTION RespInfo *FORM
Respondent Information 
1:Respondent Name *ALPHA *MANDATORY
2:Respondent Address *ALPHA *MANDATORY
4:Pin code *NUMBER *MIN 000001 *MAX 999999
5:Landline Number (optional) *NUMBER 
6:Mobile Number *NUMBER *MANDATORY *MIN 6000000000 *MAX 9999999999
7:Email Address (optional) *ALPHA

#*MIN 6000000000 *MAX 9999999999

*IF [LengthOf[RespInfo.4]!=6] *MSG "Invalid Pin code;Length should be 6"
*IF [LengthOf[RespInfo.6]!=10] *MSG "Invalid Mobile Number;Lenght should be 10"
*IF [SubStrOf[RespInfo.6,1,1]!=6 & SubStrOf[RespInfo.6,1,1]!=7 & SubStrOf[RespInfo.6,1,1]!=8 & SubStrOf[RespInfo.6,1,1]!=9] *MSG "Invalid Mobile Number;Number followed by 6/7/8/9"

*QUESTION RespName *OPEN *DUMMY2 *ADDRESS1
Respondent Name

*INCLUDE RespName RespInfo.1

*QUESTION RespAdd *OPEN *DUMMY2 *ADDRESS1
Respondent Name

*INCLUDE RespAdd RespInfo.2

*QUESTION RespMobile *OPEN *DUMMY2 *ADDRESS3
Respondent Phone Numner 

*INCLUDE RespMobile RespInfo.6

*QUESTION Q2 *MR
Q2. Do you or any of your family members work or operate a business in any of the following industries? <font color="#FF73FF">[MA]</font>
1:Banking/Financial online services (like fintech, online insurance, online lenders, digital wallets companies) 
2:Banking/Financial offline services (like traditional banks, private banks community banks, cooperative banks, savings and loan associations or working in accounts/ customer supports or any other departments of these banks)
3:IT/Computer Software/ITES/Ecommerce/KPOs/BPOs etc
4:Education / Learning – offline (like school, colleges etc)
5:Education / Learning – online (like online learning platforms, courses etc)
6:Manufacturing
7:Automotive/Engineering
8:Construction / real estate
9:Healthcare / Pharma
10:Advertising Agency
11:Market Research Agency
12:Television/Newspaper or Magazine 
98:Others (Specify) *OPEN

*IF [Q2=1 | Q2=2 | Q2=10 | Q2=11 | Q2=12] *GOTO TN

*QUESTION Q3 *SR
Q3. Have you participated in any market research activities in the last 3 months? <font color="#FF73FF">[SA]</font>
*USELIST "YN"

*IF [Q3=1] *GOTO TN

*QUESTION Q4 *NUMBER *MIN 1 *MAX 98 *DKCS "Refused" "99"
Q4. Can you please tell me your age in completed years?
Age of the respondent <font color="#FF73FF">[OE]</font>

*LIST "AgeList"
1:Less than 25 years      
2:25 to 30 years
3:31 to 40 years      
4:41 to 50 years      
5:More than 50 years  

*QUESTION Q5 *INFO *IF [Q4=99]
Respondent refused to disclose age. Interview Terminated.

*IF [Q4=99] *GOTO TN

*QUESTION AgeGroup *SR *DUMMY2
Age Group [SA]
*USELIST "AgeList"

*IF [ValueOf[Q4]<25] *INCLUDE AgeGroup [1]
*IF [(ValueOf[Q4]>=25 & ValueOf[Q4]<=30)] *INCLUDE AgeGroup [2]
*IF [(ValueOf[Q4]>=31 & ValueOf[Q4]<=40)] *INCLUDE AgeGroup [3]
*IF [(ValueOf[Q4]>=41 & ValueOf[Q4]<=50)] *INCLUDE AgeGroup [4]
*IF [ValueOf[Q4]>50 & ValueOf[Q4]<99] *INCLUDE AgeGroup [5]

#*IF [Q5=1] *INCLUDE AgeGroup [1]
#*IF [Q5=2] *INCLUDE AgeGroup [2]
#*IF [Q5=3] *INCLUDE AgeGroup [3]
#*IF [Q5=4] *INCLUDE AgeGroup [4]
#*IF [Q5=5] *INCLUDE AgeGroup [5]

*QUESTION AgeGroupInfo *INFO
AGE Group: <b><font color="#FF73FF">{AgeGroup}</font></b>

*IF [AgeGroup=1 | AgeGroup=5] *GOTO TN

*QUESTION AgeReport *SR *DUMMY2
Q5. Age of respondent
1:25 to 30 years
2:31 to 40 years      
3:41 to 50 years   

*IF [AgeGroup=2] *INCLUDE AgeReport [1]
*IF [AgeGroup=3] *INCLUDE AgeReport [2]
*IF [AgeGroup=4] *INCLUDE AgeReport [3]

*QUESTION Q6 *SR
<font color="#FF73FF">RECORD GENDER – DO NOT ASK</font>
<br>Q6. Record Gender? <font color="#FF73FF">[SA]</font>
1:Male
2:Female

*QUESTION Q7 *SR
Q7. Are you the <b>Chief Wage Earner</b> of the family?
By Chief wage earner we mean the person in a household who contributes maximum towards the household expenditure. <font color="#FF73FF">[SA]</font>
*USELIST "YN"

*QUESTION Q8 *SR
[SHOW SCREEN]
<br>Now I would like to ask you something about the Chief Wage Earner of the family. 
By Chief Wage Earner, I mean that person in your household who contributes the maximum to the monthly household expenditure.
<br>Q8. What is his/her highest educational level? <font color="#FF73FF">[SA]</font>
1:Illiterate 
2:Literate but no formal schooling 
3:School up to 4 years 
4:School 5 to 9 years 
5:SSC / HSC 
6:Some College (includes a Diploma) but not Grad  
7:Graduate / Post Graduate - General (e.g. BA/ BSc/ B.Com/ MA/ MSc/ M.Com) 
8:Graduate / Post Graduate - Professional (e.g. B. Tech/ B. Arch/ M. Tech/ MBBS/ CA/ MBA/ etc.) 

*QUESTION Q9 *MR
[SHOW SCREEN]
<br>We have a standard list of items that we use in all kinds of cities and villages. So don't worry if an item appears irrelevant for you or too ordinary-just go ahead and tell me which items you do have household. We need this information just for survey purpose only.
<br>Q9. Please take a look at this list and tell me which of these <b>items do you have at home?</b> It could be <b>owned by you, your family or provided by the employer or it could be available in the house you live in;</b> but it should be for the <b>use of just you or your family</b> <font color="#FF73FF">[MA]</font>
1:Air Conditioner   
2:Fridge - Refrigerator (One door or two door)
3:Washing Machine  
4:Color TV/LCD/LED/Plasma TV   
5:Personal Computer/ Laptop  
6:Electricity Connection   
7:Ceiling Fan   
8:LPG Stove  
9:Two-Wheeler   
10:Car/Jeep/Van (Four-Wheeler)  
11:Agricultural Land 
99:None *NMUL 

*IF [Q9!=6] *MSG "Electricity Connection should be selected"


*QUESTION Q9a *NUMBER *DUMMY2
Q10. Total Number of household durables

*IF [Q9=99] *INCLUDE Q9a [0]
*IF [Q9!=99 & NumberOfResponse[Q9]=1]  *INCLUDE  Q9a [1]
*IF [Q9!=99 & NumberOfResponse[Q9]=2]  *INCLUDE  Q9a [2]
*IF [Q9!=99 & NumberOfResponse[Q9]=3]  *INCLUDE  Q9a [3]
*IF [Q9!=99 & NumberOfResponse[Q9]=4]  *INCLUDE  Q9a [4]
*IF [Q9!=99 & NumberOfResponse[Q9]=5]  *INCLUDE  Q9a [5]
*IF [Q9!=99 & NumberOfResponse[Q9]=6]  *INCLUDE  Q9a [6]
*IF [Q9!=99 & NumberOfResponse[Q9]=7]  *INCLUDE  Q9a [7]
*IF [Q9!=99 & NumberOfResponse[Q9]=8]  *INCLUDE  Q9a [8]
*IF [Q9!=99 & NumberOfResponse[Q9]=9]  *INCLUDE  Q9a [9]
*IF [Q9!=99 & NumberOfResponse[Q9]=10] *INCLUDE  Q9a [10]
*IF [Q9!=99 & NumberOfResponse[Q9]=11] *INCLUDE  Q9a [11]

*QUESTION SEC *SR *DUMMY2
New SEC [SA]
1:NCCS A1                                                            
2:NCCS A2                                                            
3:NCCS A3                                                           
4:NCCS B1                                                            
5:NCCS B2                                                            
6:NCCS C1                                                            
7:NCCS C2
8:NCCS D1
9:NCCS D2
10:NCCS E1
11:NCCS E2
12:NCCS E3

##
*IF [Q8=1 & Q9a=0] *INCLUDE SEC [12]
*IF [Q8=1 & Q9a=1] *INCLUDE SEC [11]
*IF [Q8=1 & Q9a=2] *INCLUDE SEC [10]
*IF [Q8=1 & Q9a=3] *INCLUDE SEC [9]
*IF [Q8=1 & Q9a=4] *INCLUDE SEC [8]
*IF [Q8=1 & Q9a=5] *INCLUDE SEC [7]
*IF [Q8=1 & Q9a=6] *INCLUDE SEC [6]
*IF [Q8=1 & Q9a=7] *INCLUDE SEC [6]
*IF [Q8=1 & Q9a=8] *INCLUDE SEC [4]
*IF [Q8=1 & Q9a=9] *INCLUDE SEC [4]
*IF [Q8=1 & Q9a=10] *INCLUDE SEC [4]
*IF [Q8=1 & Q9a=11] *INCLUDE SEC [4]

*IF [Q8=2 & Q9a=0] *INCLUDE SEC [11]
*IF [Q8=2 & Q9a=1] *INCLUDE SEC [10]
*IF [Q8=2 & Q9a=2] *INCLUDE SEC [10]
*IF [Q8=2 & Q9a=3] *INCLUDE SEC [9]
*IF [Q8=2 & Q9a=4] *INCLUDE SEC [7]
*IF [Q8=2 & Q9a=5] *INCLUDE SEC [6]
*IF [Q8=2 & Q9a=6] *INCLUDE SEC [5]
*IF [Q8=2 & Q9a=7] *INCLUDE SEC [4]
*IF [Q8=2 & Q9a=8] *INCLUDE SEC [3]
*IF [Q8=2 & Q9a=9] *INCLUDE SEC [3]
*IF [Q8=2 & Q9a=10] *INCLUDE SEC [3]
*IF [Q8=2 & Q9a=11] *INCLUDE SEC [3]

*IF [Q8=3 & Q9a=0] *INCLUDE SEC [11]
*IF [Q8=3 & Q9a=1] *INCLUDE SEC [10]
*IF [Q8=3 & Q9a=2] *INCLUDE SEC [10]
*IF [Q8=3 & Q9a=3] *INCLUDE SEC [9]
*IF [Q8=3 & Q9a=4] *INCLUDE SEC [7]
*IF [Q8=3 & Q9a=5] *INCLUDE SEC [6]
*IF [Q8=3 & Q9a=6] *INCLUDE SEC [5]
*IF [Q8=3 & Q9a=7] *INCLUDE SEC [4]
*IF [Q8=3 & Q9a=8] *INCLUDE SEC [3]
*IF [Q8=3 & Q9a=9] *INCLUDE SEC [3]
*IF [Q8=3 & Q9a=10] *INCLUDE SEC [3]
*IF [Q8=3 & Q9a=11] *INCLUDE SEC [3]


*IF [Q8=4 & Q9a=0] *INCLUDE SEC [11]
*IF [Q8=4 & Q9a=1] *INCLUDE SEC [10]
*IF [Q8=4 & Q9a=2] *INCLUDE SEC [9]
*IF [Q8=4 & Q9a=3] *INCLUDE SEC [8]
*IF [Q8=4 & Q9a=4] *INCLUDE SEC [7]
*IF [Q8=4 & Q9a=5] *INCLUDE SEC [6]
*IF [Q8=4 & Q9a=6] *INCLUDE SEC [5]
*IF [Q8=4 & Q9a=7] *INCLUDE SEC [4]
*IF [Q8=4 & Q9a=8] *INCLUDE SEC [3]
*IF [Q8=4 & Q9a=9] *INCLUDE SEC [3]
*IF [Q8=4 & Q9a=10] *INCLUDE SEC [3]
*IF [Q8=4 & Q9a=11] *INCLUDE SEC [3]

*IF [Q8=5 & Q9a=0] *INCLUDE SEC [11]
*IF [Q8=5 & Q9a=1] *INCLUDE SEC [10]
*IF [Q8=5 & Q9a=2] *INCLUDE SEC [9]
*IF [Q8=5 & Q9a=3] *INCLUDE SEC [8]
*IF [Q8=5 & Q9a=4] *INCLUDE SEC [6]
*IF [Q8=5 & Q9a=5] *INCLUDE SEC [5]
*IF [Q8=5 & Q9a=6] *INCLUDE SEC [4]
*IF [Q8=5 & Q9a=7] *INCLUDE SEC [3]
*IF [Q8=5 & Q9a=8] *INCLUDE SEC [3]
*IF [Q8=5 & Q9a=9] *INCLUDE SEC [2]
*IF [Q8=5 & Q9a=10] *INCLUDE SEC [2]
*IF [Q8=5 & Q9a=11] *INCLUDE SEC [2]

*IF [Q8=6 & Q9a=0] *INCLUDE SEC [11]
*IF [Q8=6 & Q9a=1] *INCLUDE SEC [9]
*IF [Q8=6 & Q9a=2] *INCLUDE SEC [8]
*IF [Q8=6 & Q9a=3] *INCLUDE SEC [7]
*IF [Q8=6 & Q9a=4] *INCLUDE SEC [6]
*IF [Q8=6 & Q9a=5] *INCLUDE SEC [4]
*IF [Q8=6 & Q9a=6] *INCLUDE SEC [3]
*IF [Q8=6 & Q9a=7] *INCLUDE SEC [3]
*IF [Q8=6 & Q9a=8] *INCLUDE SEC [2]
*IF [Q8=6 & Q9a=9] *INCLUDE SEC [2]
*IF [Q8=6 & Q9a=10] *INCLUDE SEC [2]
*IF [Q8=6 & Q9a=11] *INCLUDE SEC [2]

*IF [Q8=7 & Q9a=0] *INCLUDE SEC [10]
*IF [Q8=7 & Q9a=1] *INCLUDE SEC [9]
*IF [Q8=7 & Q9a=2] *INCLUDE SEC [8]
*IF [Q8=7 & Q9a=3] *INCLUDE SEC [7]
*IF [Q8=7 & Q9a=4] *INCLUDE SEC [5]
*IF [Q8=7 & Q9a=5] *INCLUDE SEC [4]
*IF [Q8=7 & Q9a=6] *INCLUDE SEC [3]
*IF [Q8=7 & Q9a=7] *INCLUDE SEC [2]
*IF [Q8=7 & Q9a=8] *INCLUDE SEC [2]
*IF [Q8=7 & Q9a=9] *INCLUDE SEC [1]
*IF [Q8=7 & Q9a=10] *INCLUDE SEC [1]
*IF [Q8=7 & Q9a=11] *INCLUDE SEC [1]

*IF [Q8=8 & Q9a=0] *INCLUDE SEC [10]
*IF [Q8=8 & Q9a=1] *INCLUDE SEC [9]
*IF [Q8=8 & Q9a=2] *INCLUDE SEC [8]
*IF [Q8=8 & Q9a=3] *INCLUDE SEC [7]
*IF [Q8=8 & Q9a=4] *INCLUDE SEC [5]
*IF [Q8=8 & Q9a=5] *INCLUDE SEC [4]
*IF [Q8=8 & Q9a=6] *INCLUDE SEC [3]
*IF [Q8=8 & Q9a=7] *INCLUDE SEC [2]
*IF [Q8=8 & Q9a=8] *INCLUDE SEC [2]
*IF [Q8=8 & Q9a=9] *INCLUDE SEC [1]
*IF [Q8=8 & Q9a=10] *INCLUDE SEC [1]
*IF [Q8=8 & Q9a=11] *INCLUDE SEC [1]

*IF [Q8=9 & Q9a=0] *INCLUDE SEC [10]
*IF [Q8=9 & Q9a=1] *INCLUDE SEC [9]
*IF [Q8=9 & Q9a=2] *INCLUDE SEC [8]
*IF [Q8=9 & Q9a=3] *INCLUDE SEC [7]
*IF [Q8=9 & Q9a=4] *INCLUDE SEC [5]
*IF [Q8=9 & Q9a=5] *INCLUDE SEC [4]
*IF [Q8=9 & Q9a=6] *INCLUDE SEC [3]
*IF [Q8=9 & Q9a=7] *INCLUDE SEC [2]
*IF [Q8=9 & Q9a=8] *INCLUDE SEC [2]
*IF [Q8=9 & Q9a=9] *INCLUDE SEC [1]
*IF [Q8=9 & Q9a=10] *INCLUDE SEC [1]
*IF [Q8=9 & Q9a=11] *INCLUDE SEC [1]


*QUESTION SECInfo *INFO *FONTSIZE 18
SEC : <b><font color="#FF73FF">{SEC}</font></b>

*IF [ValueOf[SEC]>=5 & ValueOf[SEC]<=12] *GOTO TN


*QUESTION SecReport *SR *DUMMY2
SEC
1:A                                                           
2:B1

*IF [SEC=1 | SEC=2 | SEC=3] *INCLUDE SecReport [1]
*IF [SEC=4] *INCLUDE SecReport [2]

*QUESTION Q10 *SR
Q10. What is your occupation? <font color="#FF73FF">[SA]</font>
1:Unskilled Worker
2:Skilled Worker
3:Petty Trader
4:Shop Owner
5:Businessman/Industrialist with no of employees: None
6:Businessman/Industrialist with no of employees: 1-9
7:Businessman/Industrialist with no of employees: 10+
8:Self Employed Professional
9:Clerical / Salesman
10:Supervisory Level
11:Officer Executive – Junior
12:Officer Executive - Middle / Senior
13:Student
14:Unemployed
98:Others *OPEN

*IF [Q10=1 | Q10=2 | Q10=3 | Q10=13 | Q10=14] *GOTO TN

*QUESTION HD10 *SR *DUMMY2
Occupation auto code (Hidden in the Live Link)
1:Business/Professional
2:Salaried

*IF [Q10=4 | Q10=5 | Q10=6 | Q10=7 | Q10=8] *INCLUDE HD10 [1]
*IF [Q10=9 | Q10=10 | Q10=11 | Q10=12] *INCLUDE HD10 [2]

*QUESTION HD10Info *INFO
HD10 Info:<b>{HD10}</b>

*QUESTION Q11 *SR
Q11. Are you the key decision maker on financial matters for yourself/ your family? Please select the most appropriate option. <font color="#FF73FF">[SA]</font>  
1:Yes, I am the key decision maker for making any finance related decisions for myself/ Family
2:Yes, I am one of the key decision makers for making any finance related decisions for myself / Family 
3:No, I am not involved in any finance related decision making for myself/ Family

*IF [Q11=3] *GOTO TN

#####################################################USERSHIP##############################################

*LIST "Q12Listabc"
1:Credit Card
2:Debit Card
3:UPI
4:Personal loan
5:Home Loan
6:2-wheeler loan
7:4-wheeler loan
8:2/4 wheeler insurance
9:Term insurance
10:IPO, Stock, Equity, Bonds
11:FD, PPF, Kisan Vikas Patra
12:SIP, MF, ELSS, ETF's
13:NPS (National Pension Scheme)

*STARTREC "Q12abc"

*QUESTION Q12a *MR *RANDOM
[SHOW SCREEN]
<br>Q12a. Which of these <b>financial products</b> have you taken for <b>yourself in past 12 months?</b> Please select all that applies. <font color="#FF73FF">[MA]</font> 
*USELIST "Q12Listabc" 
99:None of the above *NOCON *NMUL

#*INCLUDE [Q12a] *IF [NumberOfResponse[Q12a]>0]

*QUESTION Q12b *MR *RANDOM 
[SHOW SCREEN]
<br>Q12b. And, which of these <b>financial products</b> do you currently own for <b>yourself in active condition?</b> Please select all that applies. <font color="#FF73FF">[MA]</font> 
*USELIST "Q12Listabc"
99:None of the above *NOCON *NMUL

#*IF [NumberOfResponse[Q12b]>0]

*QUESTION Q12c *MR *RANDOM
[SHOW SCREEN]
<br>Q12c. Which of these <b>financial products</b> are you planning to take for yourself in <b>next 6 months?</b>  <font color="#FF73FF">[MA]</font> 
*USELIST "Q12Listabc"
99:None of the above *NOCON *NMUL

*IF [Q12a=99 & Q12b=99 & Q12c=99] *GOTO TN

*LIST "Q12dList"
1:Only User
2:User+ Intender
3:First Time intender
4:Any other

*QUESTION DummyQHD12d *SR *DUMMY2
QHD12d. Usership auto code (Display in the Live Link, No change in response allowed)
*USELIST "Q12dList"

*IF [Q12b=1 & Q12c!=1] *INCLUDE DummyQHD12d [1]
*IF [Q12b=1 & Q12c=1] *INCLUDE DummyQHD12d [2]
*IF [Q12a!=1 & Q12b!=1 & Q12c=1] *INCLUDE DummyQHD12d [3]

*QUESTION QHD12d *SR *DUMMY2
QHD12d. Usership auto code (Display in the Live Link, No change in response allowed)
*USELIST "Q12dList"

*IF [NumberOfResponse[DummyQHD12d]>0] *INCLUDE QHD12d DummyQHD12d
*IF [NumberOfResponse[DummyQHD12d]=0] *INCLUDE QHD12d [4]


*QUESTION QHD12dInfo *INFO
Usership : {QHD12d}

*IF [QHD12d=4] *GOTO TN

*QUESTION UserReport *SR *DUMMY2
Usership
1:Owner
2:Intender of CC (N6M)

*IF [QHD12d=1 | QHD12d=2] *INCLUDE UserReport [1]
*IF [QHD12d=3] *INCLUDE UserReport [2]

*ENDREC

################################################################################################

*LIST "Q12efList"
1:Last 1 year
2:1.1-3 years
3:3.1-5 years
4:5.1-7 years
5:7.1-10 years
6:10.1-12 years
7:12.1-15 years
8:More than 15 years

*QUESTION Q12e *SR *IF [Q12a=1 | Q12b=1]
Q12e. When did you start using your first <b>Credit card</b> for yourself? (SA)
*USELIST "Q12efList"

*QUESTION Q12f *SR *IF [Q12a=2 | Q12b=2]
Q12f. When did you start using your first <b>Debit card</b> for yourself? (SA)
*USELIST "Q12efList"

######################################################BRAND SALIENCY ################################################################

*STARTREC "A13abc"

*LIST "Q13abcList"
1:American Express Bank
2:Axis Bank
3:Bandhan Bank
4:Bank of Baroda
5:Bank of India
6:Bank of Maharashtra
7:Canara Bank
8:Citibank
9:DBS 
10:HDFC Bank
11:HSBC
12:ICICI Bank
13:IDFC First Bank
14:IndusInd Bank
15:Kotak Mahindra Bank
16:Punjab National Bank
17:Standard Chartered Bank
18:State Bank of India / SBI
19:YES Bank

*QUESTION Q13a *SR *COLUMN 2
Q13a. Thinking of <b>Banks that offer Credit Cards,</b> which all banks come to your mind? <b>[SA]</b> <font color="#FF73FF">DO NOT PROMPT OR SHOW SCREEN.</font>
<br><font color="#FF73FF">INTERVIEWER : CODE THE VERY FIRST BRAND MENTIONED IN Q13a AND REMAINING BRANDS UNDER Q13b</font>
*USELIST "Q13abcList"
95:Others please specify *OPEN
99:None *NMUL

*QUESTION Q13b *MR *EXCLUDE [Q13a] *COLUMN 2
Q13b. Which <b>other banks that offer credit Cards</b> that you are aware of ? <b>[MA]</b> <font color="#FF73FF">DO NOT SHOW LIST</font>
*USELIST "Q13abcList"
96:Others please specify *OPEN
99:None *NOCON *NMUL

*QUESTION TomSpont *MR *DUMMY2
TOM SPONT
*USELIST "Q13abcList"
95:Others
96:Others
99:None

*INCLUDE TomSpont Q13a
*INCLUDE TomSpont Q13b

*LIST "Q13abcListImg"
1:American Express Bank *PICT "01AEB.jpg"
2:Axis Bank *PICT "02AB.jpg" 
3:Bandhan Bank *PICT "03BdhB.jpg"
4:Bank of Baroda *PICT "04BB.jpg"
5:Bank of India *PICT "05BI.jpg"
6:Bank of Maharashtra *PICT "06BM.jpg"
7:Canara Bank *PICT "07CB.jpg"
8:Citibank *PICT "08CB.jpg"
9:DBS *PICT "09DBS.jpg"
10:HDFC Bank *PICT "10HDFCB.jpg"
11:HSBC *PICT "11HSBCB.jpg"
12:ICICI Bank *PICT "12ICICIB.jpg"
13:IDFC First Bank *PICT "13IDFCFB.jpg"
14:IndusInd Bank *PICT "14IB.jpg"
15:Kotak Mahindra Bank *PICT "15KB.jpg"
16:Punjab National Bank *PICT "16PNB.jpg"
17:Standard Chartered Bank *PICT "17SCB.jpg"
18:State Bank of India / SBI *PICT "18SBI.jpg"
19:YES Bank *PICT "19YB.jpg"

*QUESTION Q13c *MR *RANDOM *EXCLUDE [TomSpont] *COLUMN 2
[SHOW LOGO CARD TO THE RESPONDENT]
<br>Q13c. Which of these <b>banks that offer credit cards</b> are you <b>aware</b> of? <font color="#FF73FF">[MA]</font>
*USELIST "Q13abcListImg"
99:None *NMUL *NOCON

*IF [Q13a=99 & Q13b=99 & Q13c=99] *GOTO TN

*QUESTION Q13abc *MR *DUMMY2
QHD13d. TOTAL AWARENESS
*USELIST "Q13abcList"
95:Others
96:Others
99:None

*INCLUDE Q13abc TomSpont
*INCLUDE Q13abc Q13c

*QUESTION Q13abcCnt *MR *DUMMY2
QHD13d. TOTAL AWARENESS
*USELIST "Q13abcList"

*INCLUDE Q13abcCnt [0]
*IF [ValueOf[Q13abc]>=1 & ValueOf[Q13abc]<=19] *INCLUDE Q13abcCnt [1]
*IF [NumberOfResponse[Q13abcCnt]=0] *GOTO TN

*ENDREC

*STARTREC "Q14abc"
*QUESTION Q14a *MR *INCLUDE [Q13abc] *COLUMN 2 *RANDOM *IF [(QHD12d=1 | QHD12d=2) & NumberOfResponse[Q13abc]>0]
[SHOW SCREEN TO THE RESPONDENT]
<br>Q14a. From which of these banks, have you <b>ever owned</b> a Credit Cards for yourself? <font color="#FF73FF">[MA]</font>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}

*IF [NumberOfResponse[Q14a]>=4] *MSG "PROMPT:PLEASE INDICATE ONLY THE CREDIT CARDS THAT YOU YOURSELF OWN. THANK YOU."

#*INCLUDE [Q14a] (QHD12d=1 | QHD12d=2) &

*QUESTION Q14b *MR *COLUMN 2 *INCLUDE [Q14a] *IF [(QHD12d=1 | QHD12d=2) & NumberOfResponse[Q14a]>0]
[SHOW SCREEN TO THE RESPONDENT]
<br>Q14b. From which of these banks have you <b>taken</b> a Credit Card for yourself in the past <b>12 months?</b> <font color="#FF73FF">[MA]</font>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
99:None *NMUL *NOCON

*IF [NumberOfResponse[Q14b]>=4] *MSG "PROMPT:PLEASE INDICATE ONLY THE CREDIT CARDS THAT YOU YOURSELF OWN. THANK YOU."

#& (QHD12d=1 | QHD12d=2)

*QUESTION Q14c *MR *INCLUDE [Q14a] *COLUMN 2 *IF [(QHD12d=1 | QHD12d=2) & Q14b!=99 & NumberOfResponse[Q14a]>0]
[SHOW SCREEN TO THE RESPONDENT]
<br>Q14c. And, from which of these banks have you taken a Credit Card for yourself in the past <b>3 months?</b> <font color="#FF73FF">[MA]</font>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
99:None *NMUL *NOCON

*IF [NumberOfResponse[Q14c]>=4] *MSG "PROMPT:PLEASE INDICATE ONLY THE CREDIT CARDS THAT YOU YOURSELF OWN. THANK YOU."


*ENDREC

#*INCLUDE [Q13abc]

*QUESTION DummyQ14d *MR *DUMMY2
DummyQ14d
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}

*INCLUDE DummyQ14d [1;2;3;4;5;6;7;8;9;10;11;12;13;14;15;16;17;18;19]
*IF [Q13a=95] *INCLUDE DummyQ14d [95]
*IF [Q13b=96] *INCLUDE DummyQ14d [96]
*IF [Q13b=97] *INCLUDE DummyQ14d [97]

*QUESTION Q14d *MR *INCLUDE [DummyQ14d] *COLUMN 2 *RANDOM
[SHOW SCREEN TO THE RESPONDENT]
<br>Q14d. With Which all banks do you currently have <b>active saving/ salary account?</b> <font color="#FF73FF">[MA]</font>
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
996:Other1 (Specify) *OPEN *NOCON
997:Other2 (Specify) *OPEN *NOCON
998:Other3 (Specify) *OPEN *NOCON

*IF [NumberOfResponse[Q14d]>=4] *MSG "PROMPT:PLEASE INDICATE ONLY THE CREDIT CARDS THAT YOU YOURSELF OWN. THANK YOU."

#############################################################################

*IF [QHD12d=3] *GOTO Q17b1

*QUESTION Q15 *SR 
Q15. Can you tell me total number of credit cards that are currently active with you? <font color="#FF73FF">[SA]</font>
1:1
2:2
3:3
4:4
5:5
6:6
7:7+
99:None

*IF [Q15=99] *GOTO TN

*QUESTION Q15No *MR *DUMMY2
Number of Card
1:1
2:2
3:3
4:4
5:5
6:6
7:7

*IF [Q15=1] *INCLUDE Q15No [1]
*IF [Q15=2] *INCLUDE Q15No [1;2]
*IF [Q15=3] *INCLUDE Q15No [1;2;3]
*IF [Q15=4] *INCLUDE Q15No [1;2;3;4]
*IF [Q15=5] *INCLUDE Q15No [1;2;3;4;5]
*IF [Q15=6] *INCLUDE Q15No [1;2;3;4;5;6]
*IF [Q15=7] *INCLUDE Q15No [1;2;3;4;5;6;7]

*GRIDLIST "Q16List"
1:American Express Bank
2:Axis Bank
3:Bandhan Bank
4:Bank of Baroda
5:Bank of India
6:Bank of Maharashtra
7:Canara Bank
8:Citibank
9:DBS 
10:HDFC Bank
11:HSBC
12:ICICI Bank
13:IDFC First Bank
14:IndusInd Bank
15:Kotak Mahindra Bank
16:Punjab National Bank
17:Standard Chartered Bank
18:State Bank of India / SBI
19:YES Bank
95:{Q13a.95}
96:{Q13b.96}

*STARTREC "Q16a"

*LIST "CardNList"
1:Card 1
2:Card 2
3:Card 3
4:Card 4
5:Card 5
6:Card 6
7:Card 7+

*QUESTION Q16a *GRIDSR *USEGRIDLIST "Q16List" *INCLUDEGRIDLIST [Q14a] *INCLUDE [Q15No] *IF [NumberOfResponse[Q15No]>0]
Q16a. Please tell me the banks of each of these Credit Cards <b>Currently Active</b> for your usage? <font color="#FF73FF">[SA]</font>
*USELIST "CardNList"

*QUESTION Q16bDummy *MR *DUMMY2
Q16bDummy
*USELIST "Q13abcList"
95:Others
96:Others

*IF [ValueOf[Q16a.1]=1  | ValueOf[Q16a.2]=1  | ValueOf[Q16a.3]=1  | ValueOf[Q16a.4]=1  | ValueOf[Q16a.5]=1  | ValueOf[Q16a.6]=1  | ValueOf[Q16a.7]=1] *INCLUDE Q16bDummy [1]
*IF [ValueOf[Q16a.1]=2  | ValueOf[Q16a.2]=2  | ValueOf[Q16a.3]=2  | ValueOf[Q16a.4]=2  | ValueOf[Q16a.5]=2  | ValueOf[Q16a.6]=2  | ValueOf[Q16a.7]=2] *INCLUDE Q16bDummy [2]
*IF [ValueOf[Q16a.1]=3  | ValueOf[Q16a.2]=3  | ValueOf[Q16a.3]=3  | ValueOf[Q16a.4]=3  | ValueOf[Q16a.5]=3  | ValueOf[Q16a.6]=3  | ValueOf[Q16a.7]=3] *INCLUDE Q16bDummy [3]
*IF [ValueOf[Q16a.1]=4  | ValueOf[Q16a.2]=4  | ValueOf[Q16a.3]=4  | ValueOf[Q16a.4]=4  | ValueOf[Q16a.5]=4  | ValueOf[Q16a.6]=4  | ValueOf[Q16a.7]=4] *INCLUDE Q16bDummy [4]
*IF [ValueOf[Q16a.1]=5  | ValueOf[Q16a.2]=5  | ValueOf[Q16a.3]=5  | ValueOf[Q16a.4]=5  | ValueOf[Q16a.5]=5  | ValueOf[Q16a.6]=5  | ValueOf[Q16a.7]=5] *INCLUDE Q16bDummy [5]
*IF [ValueOf[Q16a.1]=6  | ValueOf[Q16a.2]=6  | ValueOf[Q16a.3]=6  | ValueOf[Q16a.4]=6  | ValueOf[Q16a.5]=6  | ValueOf[Q16a.6]=6  | ValueOf[Q16a.7]=6] *INCLUDE Q16bDummy [6]
*IF [ValueOf[Q16a.1]=7  | ValueOf[Q16a.2]=7  | ValueOf[Q16a.3]=7  | ValueOf[Q16a.4]=7  | ValueOf[Q16a.5]=7  | ValueOf[Q16a.6]=7  | ValueOf[Q16a.7]=7] *INCLUDE Q16bDummy [7]
*IF [ValueOf[Q16a.1]=8  | ValueOf[Q16a.2]=8  | ValueOf[Q16a.3]=8  | ValueOf[Q16a.4]=8  | ValueOf[Q16a.5]=8  | ValueOf[Q16a.6]=8  | ValueOf[Q16a.7]=8] *INCLUDE Q16bDummy [8]
*IF [ValueOf[Q16a.1]=9  | ValueOf[Q16a.2]=9  | ValueOf[Q16a.3]=9  | ValueOf[Q16a.4]=9  | ValueOf[Q16a.5]=9  | ValueOf[Q16a.6]=9  | ValueOf[Q16a.7]=9] *INCLUDE Q16bDummy [9]
*IF [ValueOf[Q16a.1]=10 | ValueOf[Q16a.2]=10 | ValueOf[Q16a.3]=10 | ValueOf[Q16a.4]=10 | ValueOf[Q16a.5]=10 | ValueOf[Q16a.6]=10 | ValueOf[Q16a.7]=10] *INCLUDE Q16bDummy [10]
*IF [ValueOf[Q16a.1]=11 | ValueOf[Q16a.2]=11 | ValueOf[Q16a.3]=11 | ValueOf[Q16a.4]=11 | ValueOf[Q16a.5]=11 | ValueOf[Q16a.6]=11 | ValueOf[Q16a.7]=11] *INCLUDE Q16bDummy [11]
*IF [ValueOf[Q16a.1]=12 | ValueOf[Q16a.2]=12 | ValueOf[Q16a.3]=12 | ValueOf[Q16a.4]=12 | ValueOf[Q16a.5]=12 | ValueOf[Q16a.6]=12 | ValueOf[Q16a.7]=12] *INCLUDE Q16bDummy [12]
*IF [ValueOf[Q16a.1]=13 | ValueOf[Q16a.2]=13 | ValueOf[Q16a.3]=13 | ValueOf[Q16a.4]=13 | ValueOf[Q16a.5]=13 | ValueOf[Q16a.6]=13 | ValueOf[Q16a.7]=13] *INCLUDE Q16bDummy [13]
*IF [ValueOf[Q16a.1]=14 | ValueOf[Q16a.2]=14 | ValueOf[Q16a.3]=14 | ValueOf[Q16a.4]=14 | ValueOf[Q16a.5]=14 | ValueOf[Q16a.6]=14 | ValueOf[Q16a.7]=14] *INCLUDE Q16bDummy [14]
*IF [ValueOf[Q16a.1]=15 | ValueOf[Q16a.2]=15 | ValueOf[Q16a.3]=15 | ValueOf[Q16a.4]=15 | ValueOf[Q16a.5]=15 | ValueOf[Q16a.6]=15 | ValueOf[Q16a.7]=15] *INCLUDE Q16bDummy [15]
*IF [ValueOf[Q16a.1]=16 | ValueOf[Q16a.2]=16 | ValueOf[Q16a.3]=16 | ValueOf[Q16a.4]=16 | ValueOf[Q16a.5]=16 | ValueOf[Q16a.6]=16 | ValueOf[Q16a.7]=16] *INCLUDE Q16bDummy [16]
*IF [ValueOf[Q16a.1]=17 | ValueOf[Q16a.2]=17 | ValueOf[Q16a.3]=17 | ValueOf[Q16a.4]=17 | ValueOf[Q16a.5]=17 | ValueOf[Q16a.6]=17 | ValueOf[Q16a.7]=17] *INCLUDE Q16bDummy [17]
*IF [ValueOf[Q16a.1]=18 | ValueOf[Q16a.2]=18 | ValueOf[Q16a.3]=18 | ValueOf[Q16a.4]=18 | ValueOf[Q16a.5]=18 | ValueOf[Q16a.6]=18 | ValueOf[Q16a.7]=18] *INCLUDE Q16bDummy [18]
*IF [ValueOf[Q16a.1]=19 | ValueOf[Q16a.2]=19 | ValueOf[Q16a.3]=19 | ValueOf[Q16a.4]=19 | ValueOf[Q16a.5]=19 | ValueOf[Q16a.6]=19 | ValueOf[Q16a.7]=19] *INCLUDE Q16bDummy [19]
*IF [ValueOf[Q16a.1]=95 | ValueOf[Q16a.2]=95 | ValueOf[Q16a.3]=95 | ValueOf[Q16a.4]=95 | ValueOf[Q16a.5]=95 | ValueOf[Q16a.6]=95 | ValueOf[Q16a.7]=95] *INCLUDE Q16bDummy [95]
*IF [ValueOf[Q16a.1]=96 | ValueOf[Q16a.2]=96 | ValueOf[Q16a.3]=96 | ValueOf[Q16a.4]=96 | ValueOf[Q16a.5]=96 | ValueOf[Q16a.6]=96 | ValueOf[Q16a.7]=96] *INCLUDE Q16bDummy [96]

*ENDREC

*STARTREC "Q16bb1"

###################################1#############################
### 1 
*QUESTION DummyQ16b1 *NUMLIST *DUMMY2
Number Of Card Q16b1
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=1] *INCLUDE DummyQ16b1 ValueInPositionOf[1,7]

*QUESTION Q16b1No *NUMBER *DUMMY2
Number Of Card Q16b1

*INCLUDE Q16b1No TotalOf[DummyQ16b1]

*LIST "AECList"
215:American Express Gold Card   *PICT "215AEx.jpg"
216:American Express Membership Rewards Credit Card   *PICT "216AEx.jpg"
217:American Express Platinum Card   *PICT "217AEx.jpg"
218:American Express Platinum Reserve Credit Card   *PICT "218AEx.jpg"
219:American Express Platinum Travel Credit Card   *PICT "219AEx.jpg"
220:American Express SmartEarn Credit Card   *PICT "220AEx.jpg"

*QUESTION Q16b1 *MR *IF [Q16bDummy=1]
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.1}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.1}</big>
*USELIST "AECList"
798:Any other *OPEN

*IF [NumberOfResponse[Q16b1]!=ValueOf[Q16b1No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b1No}"

*GRIDLIST "GQ16b1"
1:CONFIRM
2:CHANGE

# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con1 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b1] *IF [NumberOfResponse[Q16b1]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "AECList"
798:{Q16b1.798} (American Express) *PICT "OtherCards.jpg"

*QUESTION NeedChange1 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange1 [2]

*IF [ValueOf[Q16b1Con1.215]=2] *INCLUDE NeedChange1 [1]
*IF [ValueOf[Q16b1Con1.216]=2] *INCLUDE NeedChange1 [1]
*IF [ValueOf[Q16b1Con1.217]=2] *INCLUDE NeedChange1 [1]
*IF [ValueOf[Q16b1Con1.218]=2] *INCLUDE NeedChange1 [1]
*IF [ValueOf[Q16b1Con1.219]=2] *INCLUDE NeedChange1 [1]
*IF [ValueOf[Q16b1Con1.220]=2] *INCLUDE NeedChange1 [1]
*IF [ValueOf[Q16b1Con1.798]=2] *INCLUDE NeedChange1 [1]

*QUESTION NeedChgInfo1 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange1]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

##############################################################################################
#### 2 
*QUESTION DummyQ16b2 *NUMLIST *DUMMY2
Number Of Card Q16b2
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=2] *INCLUDE DummyQ16b2 ValueInPositionOf[1,7]

*QUESTION Q16b2No *NUMBER *DUMMY2
Number Of Card Q16b2

*INCLUDE Q16b2No TotalOf[DummyQ16b2]

*LIST "ABCList"
1:NEO Credit Card   *PICT "001Axis.jpg"
2:AIRTEL AXIS BANK Credit Card   *PICT "002Axis.jpg"
3:AXIS BANK ACE Credit Card   *PICT "003Axis.jpg"
4:AXIS BANK ATLAS Credit Card   *PICT "004Axis.jpg"
5:AXIS BANK AURA Credit Card   *PICT "005Axis.jpg"
6:AXIS BANK FREECHARGE Credit Card   *PICT "006Axis.jpg"
7:AXIS BANK FREECHARGE PLUS Credit Card   *PICT "007Axis.jpg"
8:AXIS BANK MAGNUS Credit Card   *PICT "008Axis.jpg"
9:AXIS BANK MY WINGS Credit Card   *PICT "009Axis.jpg"
10:AXIS Bank MYZONE Credit Card   *PICT "010Axis.jpg"
11:AXIS BANK MYZONE EASY Credit Card   *PICT "011Axis.jpg"
12:AXIS BANK PRIDE PLATINUM Credit Card   *PICT "012Axis.jpg"
13:AXIS BANK PRIDE SIGNATURE Credit Card   *PICT "013Axis.jpg"
14:AXIS BANK PRIVILEGE Credit Card   *PICT "014Axis.jpg"
15:AXIS BANK RESERVE Credit Card   *PICT "015Axis.jpg"
16:AXIS BANK SELECT Credit Card   *PICT "016Axis.jpg"
17:AXIS BANK SHOPPERS STOP Credit Card   *PICT "017Axis.jpg"
18:AXIS BANK SIGNATURE CREDIT CARD WITH LIFESTYLE BENEFITS   *PICT "018Axis.jpg"
19:AXIS BANK VISTARA Credit Card   *PICT "019Axis.jpg"
20:AXIS BANK VISTARA INFINITE Credit Card   *PICT "020Axis.jpg"
21:AXIS BANK VISTARA SIGNATURE  Credit Card   *PICT "021Axis.jpg"
22:FIBE AXIS BANK Credit Card   *PICT "022Axis.jpg"
23:FLIPKART AXIS BANK Credit Card   *PICT "023Axis.jpg"
24:FLIPKART AXIS BANK SUPER ELITE Credit Card   *PICT "024Axis.jpg"
25:INDIAN OIL AXIS BANK Credit Card   *PICT "025Axis.jpg"
26:LIC AXIS BANK Credit Card   *PICT "026Axis.jpg"
27:LIC AXIS BANK PLATINUM Credit Card   *PICT "027Axis.jpg"
28:MILES AND MORE AXIS BANK Credit Card   *PICT "028Axis.jpg"
29:PLATINUM Credit Card (Axis)   *PICT "029Axis.jpg"
30:PRIVILEGE EASY Credit Card   *PICT "030Axis.jpg"
31:REWARDS Credit Card   *PICT "031Axis.jpg"
32:SAMSUNG AXIS BANK INFINITE Credit Card   *PICT "032Axis.jpg"
33:SAMSUNG AXIS BANK SIGNATURE Credit Card   *PICT "033Axis.jpg"
34:SPICEJET AXIS BANK VOYAGE BLACK Credit Card   *PICT "034Axis.jpg"
35:SPICEJET AXIS BANK VOYAGE Credit Card   *PICT "035Axis.jpg"
36:TITANIUM SMART TRAVELLER  Credit Card   *PICT "036Axis.jpg"

#*IMGADJBY 200
*QUESTION Q16b2 *MR *IF [Q16bDummy=2] *COLUMN 2 
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.2}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.2}</big>
*USELIST "ABCList"
1198:Any other *OPEN

*IF [NumberOfResponse[Q16b2]!=ValueOf[Q16b2No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b2No}"

# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con2 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b2] *IF [NumberOfResponse[Q16b2]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "ABCList"
1198:{Q16b2.1198} (Axis Bank) *PICT "OtherCards.jpg"

*QUESTION NeedChange2 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange2 [2]

*IF [ValueOf[Q16b1Con2.1]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.2]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.3]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.4]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.5]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.6]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.7]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.8]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.9]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.10]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.11]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.12]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.13]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.14]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.15]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.16]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.17]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.18]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.19]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.20]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.21]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.22]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.23]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.24]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.25]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.26]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.27]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.28]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.29]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.30]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.31]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.32]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.33]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.34]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.35]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.36]=2] *INCLUDE NeedChange2 [1]
*IF [ValueOf[Q16b1Con2.1198]=2] *INCLUDE NeedChange2 [1]

*QUESTION NeedChgInfo2 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange2]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

#### 3 
*QUESTION DummyQ16b3 *NUMLIST *DUMMY2
Number Of Card Q16b3
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=3] *INCLUDE DummyQ16b3 ValueInPositionOf[1,7]

*QUESTION Q16b3No *NUMBER *DUMMY2
Number Of Card Q16b3

*INCLUDE Q16b3No TotalOf[DummyQ16b3]

*QUESTION Q16b3 *FORM *IF [Q16bDummy=3]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.3}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.3}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA


*IF [ValueOf[Q16b3No]=2 & LengthOf[Q16b3.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b3No}"
*IF [ValueOf[Q16b3No]=3 & (LengthOf[Q16b3.2]<1 | LengthOf[Q16b3.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b3No}"


#### 4 
*QUESTION DummyQ16b4 *NUMLIST *DUMMY2
Number Of Card Q16b4
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=4] *INCLUDE DummyQ16b4 ValueInPositionOf[1,7]

*QUESTION Q16b4No *NUMBER *DUMMY2
Number Of Card Q16b4

*INCLUDE Q16b4No TotalOf[DummyQ16b4]


*QUESTION Q16b4 *FORM *IF [Q16bDummy=4]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.4}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.4}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA

*IF [ValueOf[Q16b4No]=2 & LengthOf[Q16b4.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b4No}"
*IF [ValueOf[Q16b4No]=3 & (LengthOf[Q16b4.2]<1 | LengthOf[Q16b4.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b4No}"


#### 5 
*QUESTION DummyQ16b5 *NUMLIST *DUMMY2
Number Of Card Q16b5
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=5] *INCLUDE DummyQ16b5 ValueInPositionOf[1,7]

*QUESTION Q16b5No *NUMBER *DUMMY2
Number Of Card Q16b5

*INCLUDE Q16b5No TotalOf[DummyQ16b5]

*QUESTION Q16b5 *FORM *IF [Q16bDummy=5]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.5}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.5}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA

*IF [ValueOf[Q16b5No]=2 & LengthOf[Q16b5.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b5No}"
*IF [ValueOf[Q16b5No]=3 & (LengthOf[Q16b5.2]<1 | LengthOf[Q16b5.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b5No}"


#### 6 
*QUESTION DummyQ16b6 *NUMLIST *DUMMY2
Number Of Card Q16b6
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=6] *INCLUDE DummyQ16b6 ValueInPositionOf[1,7]

*QUESTION Q16b6No *NUMBER *DUMMY2
Number Of Card Q16b6

*INCLUDE Q16b6No TotalOf[DummyQ16b6]


*QUESTION Q16b6 *FORM *IF [Q16bDummy=6]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.6}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.6}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA

*IF [ValueOf[Q16b6No]=2 & LengthOf[Q16b6.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b6No}"
*IF [ValueOf[Q16b6No]=3 & (LengthOf[Q16b6.2]<1 | LengthOf[Q16b6.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b6No}"


#### 7 
*QUESTION DummyQ16b7 *NUMLIST *DUMMY2
Number Of Card Q16b7
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=7] *INCLUDE DummyQ16b7 ValueInPositionOf[1,7]

*QUESTION Q16b7No *NUMBER *DUMMY2
Number Of Card Q16b7

*INCLUDE Q16b7No TotalOf[DummyQ16b7]

*QUESTION Q16b7 *FORM *IF [Q16bDummy=7]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.7}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.7}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA

*IF [ValueOf[Q16b7No]=2 & LengthOf[Q16b7.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b7No}"
*IF [ValueOf[Q16b7No]=3 & (LengthOf[Q16b7.2]<1 | LengthOf[Q16b7.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b7No}"

#### 8 
*QUESTION DummyQ16b8 *NUMLIST *DUMMY2
Number Of Card Q16b8
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=8] *INCLUDE DummyQ16b8 ValueInPositionOf[1,7]

*QUESTION Q16b8No *NUMBER *DUMMY2
Number Of Card Q16b8

*INCLUDE Q16b8No TotalOf[DummyQ16b8]

*LIST "CCBCList"
221:Citi Cash Back Credit Card   *PICT "221Citi.jpg"
222:Citi PremierMiles Credit Card   *PICT "222Citi.jpg"
223:Citi Prestige Credit Card   *PICT "223Citi.jpg"
224:Citi Rewards Credit Card   *PICT "224Citi.jpg"
225:First Citizen Citi Credit Card   *PICT "225Citi.jpg"
226:IKEA Family Credit Card by Citi   *PICT "226Citi.jpg"
227:IndianOil Citi Credit Card   *PICT "227Citi.jpg"
228:Paytm Credit Card   *PICT "228Citi.jpg"

#*IMGADJBY 200
*QUESTION Q16b8 *MR *IF [Q16bDummy=8]
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.8}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.8}</big>
*USELIST "CCBCList"
898:Any other *OPEN

*IF [NumberOfResponse[Q16b8]!=ValueOf[Q16b8No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b8No}"

##################
# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con8 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b8] *IF [NumberOfResponse[Q16b8]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "CCBCList"
898:{Q16b8.898}(Citi) *PICT "OtherCards.jpg"

*QUESTION NeedChange8 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange8 [2]
*IF [ValueOf[Q16b1Con8.221]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.222]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.223]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.224]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.225]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.226]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.227]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.228]=2] *INCLUDE NeedChange8 [1]
*IF [ValueOf[Q16b1Con8.898]=2] *INCLUDE NeedChange8 [1]

*QUESTION NeedChgInfo8 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange8]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>


#*IF [NumberOfResponse[NeedChange8]>1] *GOTO Q16b8




#### 9 
*QUESTION DummyQ16b9 *NUMLIST *DUMMY2
Number Of Card Q16b9
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=9] *INCLUDE DummyQ16b9 ValueInPositionOf[1,7]

*QUESTION Q16b9No *NUMBER *DUMMY2
Number Of Card Q16b9

*INCLUDE Q16b9No TotalOf[DummyQ16b9]

*QUESTION Q16b9 *FORM *IF [Q16bDummy=9]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.9}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.9}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA


*IF [ValueOf[Q16b9No]=2 & LengthOf[Q16b9.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b9No}"
*IF [ValueOf[Q16b9No]=3 & (LengthOf[Q16b9.2]<1 | LengthOf[Q16b9.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b9No}"


#### 10
*QUESTION DummyQ16b10 *NUMLIST *DUMMY2
Number Of Card Q16b10
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=10] *INCLUDE DummyQ16b10 ValueInPositionOf[1,7]

*QUESTION Q16b10No *NUMBER *DUMMY2
Number Of Card Q16b10

*INCLUDE Q16b10No TotalOf[DummyQ16b10]

*LIST "IBCCList"
78:6E Rewards - IndiGo HDFC Bank Credit Card   *PICT "078Hdfc.jpg"
79:6E Rewards XL- IndiGo HDFC Bank Credit Card   *PICT "079Hdfc.jpg"
80:All Miles Credit Card   *PICT "080Hdfc.jpg"
81:AP Accounts Payable Program   *PICT "081Hdfc.jpg"
82:AR Accounts Receivable Program   *PICT "082Hdfc.jpg"
83:Auto Insurance program   *PICT "083Hdfc.jpg"
84:Best Price Save Max HDFC Bank Credit Card   *PICT "084Hdfc.jpg"
85:Best Price Save Smart HDFC Bank Credit Card   *PICT "085Hdfc.jpg"
86:Bharat Credit Card   *PICT "086Hdfc.jpg"
87:Biz Black Metal Edition Credit Card   *PICT "087Hdfc.jpg"
88:Biz First Credit Card   *PICT "088Hdfc.jpg"
89:Biz Grow Credit Card   *PICT "089Hdfc.jpg"
90:Biz Power Credit Card   *PICT "090Hdfc.jpg"
91:Business Bharat Credit Card   *PICT "091Hdfc.jpg"
92:Business Corporate MoneyBack Credit Card   *PICT "092Hdfc.jpg"
93:Business Freedom Credit Card   *PICT "093Hdfc.jpg"
94:Business Gold Credit Card   *PICT "094Hdfc.jpg"
95:Business MoneyBack   *PICT "095Hdfc.jpg"
96:Business Platinum Credit Card   *PICT "096Hdfc.jpg"
97:Business Regalia   *PICT "097Hdfc.jpg"
98:Business Regalia First Credit Card   *PICT "098Hdfc.jpg"
99:Central Travel Account   *PICT "099Hdfc.jpg"
100:Corporate Platinum Credit Card (HDFC)  *PICT "100Hdfc.jpg"
101:Corporate Premium Credit Card   *PICT "101Hdfc.jpg"
102:CSC Small Business MoneyBack   *PICT "102Hdfc.jpg"
103:Dealer Credit Card   *PICT "103Hdfc.jpg"
104:Diners Club Black Credit Card   *PICT "104Hdfc.jpg"
105:Diners Club Black Metal Edition Credit Card   *PICT "105Hdfc.jpg"
106:Diners Club Premium Credit Card   *PICT "106Hdfc.jpg"
107:Diners Club Privilege Credit Card   *PICT "107Hdfc.jpg"
108:Diners Club Rewardz Credit Card   *PICT "108Hdfc.jpg"
109:Diners ClubMiles Credit Card   *PICT "109Hdfc.jpg"
110:Doctor's Regalia Credit Card   *PICT "110Hdfc.jpg"
111:Doctors Superia Credit Card   *PICT "111Hdfc.jpg"
112:Easy EMI Card   *PICT "112Hdfc.jpg"
113:Fleet Program   *PICT "113Hdfc.jpg"
114:Flipkart Wholesale HDFC Bank Credit Card   *PICT "114Hdfc.jpg"
115:Freedom Credit Card   *PICT "115Hdfc.jpg"
116:HDFC Bank & SAP Concur Solutions Black Corporate Credit Card   *PICT "116Hdfc.jpg"
117:HDFC Bank & SAP Concur Solutions Prime Corporate Credit Card   *PICT "117Hdfc.jpg"
118:HDFC Bank Freedom RuPay Credit Card   *PICT "118Hdfc.jpg"
119:HDFC Bank H.O.G Diners Club Credit Card   *PICT "119Hdfc.jpg"
120:HDFC Bank Harley-Davidson Diners Club Credit Card    *PICT "120Hdfc.jpg"
121:HDFC Bank RuPay Shoppers Stop Credit Card   *PICT "121Hdfc.jpg"
122:HDFC Bank Times Card Credit   *PICT "122Hdfc.jpg"
123:HDFC Bank Times Card Credit Platinum   *PICT "123Hdfc.jpg"
124:HDFC Bank UPI RuPay Biz Credit Card   *PICT "124Hdfc.jpg"
125:HDFC BANK UPI RuPay Credit Card   *PICT "125Hdfc.jpg"
126:IndianOil HDFC Bank Credit Card   *PICT "126Hdfc.jpg"
127:INFINIA Metal Edition   *PICT "127Hdfc.jpg"
128:InterMiles HDFC Bank Diners Club Credit Card   *PICT "128Hdfc.jpg"
129:InterMiles HDFC Bank Platinum Credit Card   *PICT "129Hdfc.jpg"
130:InterMiles HDFC Bank Signature Credit Card   *PICT "130Hdfc.jpg"
131:IRCTC HDFC Bank Credit Card   *PICT "131Hdfc.jpg"
132:JetPrivilege HDFC Bank Select / Titanium   *PICT "132Hdfc.jpg"
133:Marriott Bonvoy HDFC Bank Credit Card   *PICT "133Hdfc.jpg"
134:Millenia Credit Card   *PICT "134Hdfc.jpg"
135:MoneyBack Credit Card   *PICT "135Hdfc.jpg"
136:Moneyback+ Credit Card   *PICT "136Hdfc.jpg"
137:Paytm HDFC Bank Business Credit Card   *PICT "137Hdfc.jpg"
138:Paytm HDFC Bank Credit Card   *PICT "138Hdfc.jpg"
139:Paytm HDFC Bank Digital Credit Card   *PICT "139Hdfc.jpg"
140:Paytm HDFC Bank Mobile Credit Card   *PICT "140Hdfc.jpg"
141:Paytm HDFC Bank SELECT Business Credit Card   *PICT "141Hdfc.jpg"
142:Paytm HDFC Bank Select Credit Card   *PICT "142Hdfc.jpg"
143:Pine Labs HDFC Bank Credit Card   *PICT "143Hdfc.jpg"
144:Pine Labs HDFC Bank Pro Credit Card   *PICT "144Hdfc.jpg"
145:Platinum Edge Credit Card   *PICT "145Hdfc.jpg"
146:Platinum Plus Credit Card   *PICT "146Hdfc.jpg"
147:Purchase Credit Card   *PICT "147Hdfc.jpg"
148:Purchase MoneyBack Credit Card   *PICT "148Hdfc.jpg"
149:Purchase Premium Credit card   *PICT "149Hdfc.jpg"
150:Purchase Reward Card   *PICT "150Hdfc.jpg"
151:Regalia Credit Card   *PICT "151Hdfc.jpg"
152:Regalia First Credit Card   *PICT "152Hdfc.jpg"
153:Regalia Gold Credit Card   *PICT "153Hdfc.jpg"
154:Shoppers Stop Black HDFC Bank Credit Card   *PICT "154Hdfc.jpg"
155:Shoppers Stop HDFC Bank Credit Card   *PICT "155Hdfc.jpg"
156:Solitaire Credit Card   *PICT "156Hdfc.jpg"
157:Superia Credit Card   *PICT "157Hdfc.jpg"
158:Swiggy HDFC Bank Credit Card   *PICT "158Hdfc.jpg"
159:Tata Neu Infinity HDFC Bank Credit Card   *PICT "159Hdfc.jpg"
160:Tata Neu Plus HDFC Bank Credit Card   *PICT "160Hdfc.jpg"
161:Teachers Platinum Credit Card   *PICT "161Hdfc.jpg"
162:Titanium Edge Credit Card   *PICT "162Hdfc.jpg"
163:TMC Card   *PICT "163Hdfc.jpg"
164:Visa Signature Credit Card   *PICT "164Hdfc.jpg"
165:World MasterCard Credit Card   *PICT "165Hdfc.jpg"

#*IMGADJBY 200
*QUESTION Q16b10 *MR *IF [Q16bDummy=10] *COLUMN 2 
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.10}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.10}</big>
*USELIST "IBCCList"
398:Any other *OPEN

*IF [NumberOfResponse[Q16b10]!=ValueOf[Q16b10No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b10No}"



##################

# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con10 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b10] *IF [NumberOfResponse[Q16b10]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "IBCCList"
398:{Q16b10.398}(HDFC) *PICT "OtherCards.jpg"

*QUESTION NeedChange10 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange10 [2]
*IF [ValueOf[Q16b1Con10.78]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.79]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.80]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.81]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.82]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.83]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.84]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.85]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.86]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.87]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.88]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.89]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.90]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.91]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.92]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.93]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.94]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.95]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.96]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.97]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.98]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.99]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.100]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.101]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.102]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.103]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.104]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.105]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.106]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.107]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.108]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.109]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.110]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.111]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.112]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.113]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.114]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.115]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.116]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.117]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.118]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.119]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.120]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.121]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.122]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.123]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.124]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.125]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.126]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.127]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.128]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.129]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.130]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.131]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.132]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.133]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.134]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.135]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.136]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.137]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.138]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.139]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.140]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.141]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.142]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.143]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.144]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.145]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.146]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.147]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.148]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.149]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.150]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.151]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.152]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.153]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.154]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.155]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.156]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.157]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.158]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.159]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.160]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.161]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.162]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.163]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.164]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.165]=2] *INCLUDE NeedChange10 [1]
*IF [ValueOf[Q16b1Con10.398]=2] *INCLUDE NeedChange10 [1]

*QUESTION NeedChgInfo10 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange10]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

#*IF [NumberOfResponse[NeedChange10]>1] *GOTO Q16b10


#### 11
*QUESTION DummyQ16b11 *NUMLIST *DUMMY2
Number Of Card Q16b11
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=11] *INCLUDE DummyQ16b11 ValueInPositionOf[1,7]

*QUESTION Q16b11No *NUMBER *DUMMY2
Number Of Card Q16b11

*INCLUDE Q16b11No TotalOf[DummyQ16b11]

*LIST "HCCList"
212:HSBC Cashback Credit Card   *PICT "212HSBC.jpg"
213:HSBC Premier Credit Card   *PICT "213HSBC.jpg"
214:HSBC Visa Platinum Credit Card   *PICT "214HSBC.jpg"

#*IMGADJBY 200
*QUESTION Q16b11 *MR *IF [Q16bDummy=11]
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.11}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.11}</big>
*USELIST "HCCList"
698:Any other *OPEN

*IF [NumberOfResponse[Q16b11]!=ValueOf[Q16b11No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b11No}"

##################
# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con11 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b11] *IF [NumberOfResponse[Q16b11]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "HCCList"
698:{Q16b11.698} (HSBC) *PICT "OtherCards.jpg"


*QUESTION NeedChange11 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange11 [2]
*IF [ValueOf[Q16b1Con11.212]=2] *INCLUDE NeedChange11 [1]
*IF [ValueOf[Q16b1Con11.213]=2] *INCLUDE NeedChange11 [1]
*IF [ValueOf[Q16b1Con11.214]=2] *INCLUDE NeedChange11 [1]
*IF [ValueOf[Q16b1Con11.698]=2] *INCLUDE NeedChange11 [1]

*QUESTION NeedChgInfo11 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange11]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

#*IF [NumberOfResponse[NeedChange11]>1] *GOTO Q16b11


#### 12
*QUESTION DummyQ16b12 *NUMLIST *DUMMY2
Number Of Card Q16b12
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=12] *INCLUDE DummyQ16b12 ValueInPositionOf[1,7]

*QUESTION Q16b12No *NUMBER *DUMMY2
Number Of Card Q16b12

*INCLUDE Q16b12No TotalOf[DummyQ16b12]

*LIST "ICICICList"
166:Accelero   *PICT "166ICICI.jpg"
167:Amazon Pay ICICI Credit Card   *PICT "167ICICI.jpg"
168:Chennai Super Kings Credit Card   *PICT "168ICICI.jpg"
169:Coral Credit Card   *PICT "169ICICI.jpg"
170:Coral RuPay Credit Card   *PICT "170ICICI.jpg"
171:Emeralde Credit Card   *PICT "171ICICI.jpg"
172:Emeralde Private Metal Credit Card   *PICT "172ICICI.jpg"
173:Emirates   *PICT "173ICICI.jpg"
174:Expressions Card   *PICT "174ICICI.jpg"
175:HPCL Coral Visa Card   *PICT "175ICICI.jpg"
176:HPCL Super Saver Credit Card   *PICT "176ICICI.jpg"
177:ICICI Bank HPCL Super Saver RuPay Credit Card   *PICT "177ICICI.jpg"
178:MakeMyTrip ICICI Bank Platinum Credit Card   *PICT "178ICICI.jpg"
179:MakeMyTrip ICICI Bank Signature Credit Card   *PICT "179ICICI.jpg"
180:Manchester United Platinum Credit Card   *PICT "180ICICI.jpg"
181:Manchester United Signature Credit Card   *PICT "181ICICI.jpg"
182:Parakram Credit Card   *PICT "182ICICI.jpg"
183:Platinum Credit Card   *PICT "183ICICI.jpg"
184:Rubyx Credit Card   *PICT "184ICICI.jpg"
185:Sapphiro Credit Card   *PICT "185ICICI.jpg"

#*IMGADJBY 200
*QUESTION Q16b12 *MR *IF [Q16bDummy=12] *COLUMN 2 
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.12}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.12}</big>
*USELIST "ICICICList"
498:Any other *OPEN 

*IF [NumberOfResponse[Q16b12]!=ValueOf[Q16b12No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b12No}"



##################
# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con12 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b12] *IF [NumberOfResponse[Q16b12]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "ICICICList"
498:{Q16b12.498}(ICICI) *PICT "OtherCards.jpg"


*QUESTION NeedChange12 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange12 [2]
*IF [ValueOf[Q16b1Con12.166]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.167]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.168]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.169]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.170]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.171]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.172]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.173]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.174]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.175]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.176]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.177]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.178]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.179]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.180]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.181]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.182]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.183]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.184]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.185]=2] *INCLUDE NeedChange12 [1]
*IF [ValueOf[Q16b1Con12.498]=2] *INCLUDE NeedChange12 [1]

*QUESTION NeedChgInfo12 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange12]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

#*IF [NumberOfResponse[NeedChange12]>1] *GOTO Q16b12

#### 13 
*QUESTION DummyQ16b13 *NUMLIST *DUMMY2
Number Of Card Q16b13
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=13] *INCLUDE DummyQ16b13 ValueInPositionOf[1,7]

*QUESTION Q16b13No *NUMBER *DUMMY2
Number Of Card Q16b13

*INCLUDE Q16b13No TotalOf[DummyQ16b13]


*QUESTION Q16b13 *FORM *IF [Q16bDummy=13]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.13}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.13}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA



*IF [ValueOf[Q16b13No]=2 & LengthOf[Q16b13.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b13No}"
*IF [ValueOf[Q16b13No]=3 & (LengthOf[Q16b13.2]<1 | LengthOf[Q16b13.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b13No}"

#### 14 
*QUESTION DummyQ16b14 *NUMLIST *DUMMY2
Number Of Card Q16b14
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=14] *INCLUDE DummyQ16b14 ValueInPositionOf[1,7]

*QUESTION Q16b14No *NUMBER *DUMMY2
Number Of Card Q16b14

*INCLUDE Q16b14No TotalOf[DummyQ16b14]


*QUESTION Q16b14 *FORM *IF [Q16bDummy=14]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.14}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.14}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA


*IF [ValueOf[Q16b14No]=2 & LengthOf[Q16b14.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b14No}"
*IF [ValueOf[Q16b14No]=3 & (LengthOf[Q16b14.2]<1 | LengthOf[Q16b14.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b14No}"



#### 15
*QUESTION DummyQ16b15 *NUMLIST *DUMMY2
Number Of Card Q16b15
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=15] *INCLUDE DummyQ16b15 ValueInPositionOf[1,7]

*QUESTION Q16b15No *NUMBER *DUMMY2
Number Of Card Q16b15

*INCLUDE Q16b15No TotalOf[DummyQ16b15]

*LIST "KotakListImg"
186:811 DreamDifferent Credit Card   *PICT "186Kotak.jpg"
187:Best Price Credit Card   *PICT "187Kotak.jpg"
188:Biz Credit Card   *PICT "188Kotak.jpg"
189:Corporate Gold Credit Card   *PICT "189Kotak.jpg"
190:Corporate Platinum Credit Card   *PICT "190Kotak.jpg"
191:Corporate Wealth Signature Credit Card   *PICT "191Kotak.jpg"
192:IndianOil Kotak Credit Card   *PICT "192Kotak.jpg"
193:Indigo 6E Rewards Credit Card   *PICT "193Kotak.jpg"
194:Kotak 811 Credit Card   *PICT "194Kotak.jpg"
195:Kotak Indigo 6E Reqards XL Credit Card   *PICT "195Kotak.jpg"
196:Kotak Infinite Credit Card   *PICT "196Kotak.jpg"
197:Kotak UPI RuPay Credit Card   *PICT "197Kotak.jpg"
198:League Platinum Credit Card   *PICT "198Kotak.jpg"
199:Metro Kotak Credit Card   *PICT "199Kotak.jpg"
200:Mojo Platinum Credit Card   *PICT "200Kotak.jpg"
201:Myntra Kotak Credit Card   *PICT "201Kotak.jpg"
202:NRI Royale Signature Credit Card   *PICT "202Kotak.jpg"
203:Privy League Signature Credit Card   *PICT "203Kotak.jpg"
204:PVR Inox Kotak Credit Card   *PICT "204Kotak.jpg"
205:PVR Kotak Gold Credit Card   *PICT "205Kotak.jpg"
206:PVR Kotak Platinum Credit Card   *PICT "206Kotak.jpg"
207:Royal Signature Credit Card   *PICT "207Kotak.jpg"
208:Urbane Gold Credit Card   *PICT "208Kotak.jpg"
209:White Credit Card   *PICT "209Kotak.jpg"
210:White Reserve Credit Card   *PICT "210Kotak.jpg"
211:Zen Signature Credit Card   *PICT "211Kotak.jpg"

#*IMGADJBY 200
*QUESTION Q16b15 *MR *IF [Q16bDummy=15] *COLUMN 2 
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.15}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.15}</big>
*USELIST "KotakListImg"       
598:Any other *OPEN

*IF [NumberOfResponse[Q16b15]!=ValueOf[Q16b15No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b15No}"


##################


*QUESTION Q16b1Con15 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b15] *IF [NumberOfResponse[Q16b15]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "KotakListImg"      
598:{Q16b15.598} (Kotak) *PICT "OtherCards.jpg"


*QUESTION NeedChange15 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange15 [2]
*IF [ValueOf[Q16b1Con15.186]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.187]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.188]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.189]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.190]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.191]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.192]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.193]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.194]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.195]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.196]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.197]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.198]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.199]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.200]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.201]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.202]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.203]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.204]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.205]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.206]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.207]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.208]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.209]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.210]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.211]=2] *INCLUDE NeedChange15 [1]
*IF [ValueOf[Q16b1Con15.598]=2] *INCLUDE NeedChange15 [1]

*QUESTION NeedChgInfo15 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange15]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

#*IF [NumberOfResponse[NeedChange15]>1] *GOTO Q16b15


#### 16 
*QUESTION DummyQ16b16 *NUMLIST *DUMMY2
Number Of Card Q16b16
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=16] *INCLUDE DummyQ16b16 ValueInPositionOf[1,7]

*QUESTION Q16b16No *NUMBER *DUMMY2
Number Of Card Q16b16

*INCLUDE Q16b16No TotalOf[DummyQ16b16]

*QUESTION Q16b16 *FORM *IF [Q16bDummy=16]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.16}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.16}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA



*IF [ValueOf[Q16b16No]=2 & LengthOf[Q16b16.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b16No}"
*IF [ValueOf[Q16b16No]=3 & (LengthOf[Q16b16.2]<1 | LengthOf[Q16b16.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b16No}"


#### 17 
*QUESTION DummyQ16b17 *NUMLIST *DUMMY2
Number Of Card Q16b17
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=17] *INCLUDE DummyQ16b17 ValueInPositionOf[1,7]

*QUESTION Q16b17No *NUMBER *DUMMY2
Number Of Card Q16b17

*INCLUDE Q16b17No TotalOf[DummyQ16b17]


*QUESTION Q16b17 *FORM *IF [Q16bDummy=17]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.17}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.17}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA


*IF [ValueOf[Q16b17No]=2 & LengthOf[Q16b17.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b17No}"
*IF [ValueOf[Q16b17No]=3 & (LengthOf[Q16b17.2]<1 | LengthOf[Q16b17.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b17No}"



#### 18
*QUESTION DummyQ16b18 *NUMLIST *DUMMY2
Number Of Card Q16b18
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=18] *INCLUDE DummyQ16b18 ValueInPositionOf[1,7]

*QUESTION Q16b18No *NUMBER *DUMMY2
Number Of Card Q16b18

*INCLUDE Q16b18No TotalOf[DummyQ16b18]

*LIST "SBICList"
37:Aditya Birla SBI Card   *PICT "037SBI.jpg"
38:Aditya Birla SBI Card SELECT   *PICT "038SBI.jpg"
39:Air India SBI Platinum Card   *PICT "039SBI.jpg"
40:Apollo SBI Card   *PICT "040SBI.jpg"
41:BPCL SBI Card   *PICT "041SBI.jpg"
42:BPCL SBI Card OCTANE   *PICT "042SBI.jpg"
43:CASHBACK SBI Card   *PICT "043SBI.jpg"
44:Central SBI Select+ Card   *PICT "044SBI.jpg"
45:Club Vistara SBI Card   *PICT "045SBI.jpg"
46:Club Vistara SBI Card PRIME   *PICT "046SBI.jpg"
47:Doctor's SBI Card   *PICT "047SBI.jpg"
48:Doctor's SBI Card(in association with IMA)   *PICT "048SBI.jpg"
49:Etihad Guest SBI Card   *PICT "049SBI.jpg"
50:Etihad Guest SBI Premier Card   *PICT "050SBI.jpg"
51:FABINDIA SBI Card   *PICT "051SBI.jpg"
52:FABINDIA SBI Card SELECT   *PICT "052SBI.jpg"
53:IRCTC SBI CARD (on Rupay platform)   *PICT "053SBI.jpg"
54:IRCTC SBI Card Premier   *PICT "054SBI.jpg"
55:IRCTC SBI Platinum Card   *PICT "055SBI.jpg"
56:Lifestyle Home Centre SBI Card   *PICT "056SBI.jpg"
57:Lifestyle Home Centre SBI Card PRIME   *PICT "057SBI.jpg"
58:Lifestyle Home Centre SBI Card SELECT   *PICT "058SBI.jpg"
59:Max SBI Card   *PICT "059SBI.jpg"
60:Max SBI Card PRIME   *PICT "060SBI.jpg"
61:Max SBI Card SELECT   *PICT "061SBI.jpg"
62:Reliance SBI Card   *PICT "062SBI.jpg"
63:Reliance SBI Card PRIME   *PICT "063SBI.jpg"
64:SBI Card ELITE   *PICT "064SBI.jpg"
65:SBI Card Miles Elite   *PICT "065SBI.jpg"
66:SBI Card MILES   *PICT "066SBI.jpg"
67:SBI Card MILES PRIME   *PICT "067SBI.jpg"
68:SBI Card PRIME   *PICT "068SBI.jpg"
69:SBI Card PULSE   *PICT "069SBI.jpg"
70:SimplyCLICK SBI Card   *PICT "070SBI.jpg"
71:SimplySAVE UPI SBI Card   *PICT "071SBI.jpg"
72:Spar SBI Card   *PICT "072SBI.jpg"
73:Spar SBI Card PRIME   *PICT "073SBI.jpg"
74:Spar SBI Card SELECT   *PICT "074SBI.jpg"
75:Titan SBI Card Rupay   *PICT "075SBI.jpg"
76:Titan SBI Card Visa   *PICT "076SBI.jpg"
77:Yatra SBI Card   *PICT "077SBI.jpg"

#*IMGADJBY 200
*QUESTION Q16b18 *MR *IF [Q16bDummy=18] *COLUMN 2 
[SHOW SCREEN]
<br>Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.18}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.18}</big>
*USELIST "SBICList"
298:Any other *OPEN

*IF [NumberOfResponse[Q16b18]!=ValueOf[Q16b18No]] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b18No}"


##################
# *PICT "OtherCards.jpg"

*QUESTION Q16b1Con18 *GRIDSR *USEGRIDLIST "GQ16b1" *INCLUDE [Q16b18] *IF [NumberOfResponse[Q16b18]>0]
[SHOW SCREEN]
Q16b1. These are the Credit Cards which you said that you own currently. Can you have a look at the screen again and confirm if any changes are required? [SA for EACH CARD]
*USELIST "SBICList"
298:{Q16b18.298} (SBI) *PICT "OtherCards.jpg"

*QUESTION NeedChange18 *MR *DUMMY2
Need Change
*USELIST "YN"

*INCLUDE NeedChange18 [2]
*IF [ValueOf[Q16b1Con18.37]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.38]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.39]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.40]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.41]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.42]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.43]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.44]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.45]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.46]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.47]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.48]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.49]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.50]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.51]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.52]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.53]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.54]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.55]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.56]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.57]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.58]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.59]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.60]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.61]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.62]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.63]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.64]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.65]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.66]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.67]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.68]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.69]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.70]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.71]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.72]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.73]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.74]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.75]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.76]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.77]=2] *INCLUDE NeedChange18 [1]
*IF [ValueOf[Q16b1Con18.298]=2] *INCLUDE NeedChange18 [1]

*QUESTION NeedChgInfo18 *INFO *NONEXTBTN *IF [NumberOfResponse[NeedChange18]>1]
<b>Need to update the selected Credit Card. Please go back and update it accordingly.</b>

#*IF [NumberOfResponse[NeedChange18]>1] *GOTO Q16b18


#### 19 
*QUESTION DummyQ16b19 *NUMLIST *DUMMY2
Number Of Card Q16b19
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=19] *INCLUDE DummyQ16b19 ValueInPositionOf[1,7]

*QUESTION Q16b19No *NUMBER *DUMMY2
Number Of Card Q16b19

*INCLUDE Q16b19No TotalOf[DummyQ16b19]


*QUESTION Q16b19 *FORM *IF [Q16bDummy=19]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.19}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.19}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA


*IF [ValueOf[Q16b19No]=2 & LengthOf[Q16b19.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b19No}"
*IF [ValueOf[Q16b19No]=3 & (LengthOf[Q16b19.2]<1 | LengthOf[Q16b19.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b19No}"


#### 95 
*QUESTION DummyQ16b95 *NUMLIST *DUMMY2
Number Of Card Q16b95
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=95] *INCLUDE DummyQ16b95 ValueInPositionOf[1,7]

*QUESTION Q16b95No *NUMBER *DUMMY2
Number Of Card Q16b95

*INCLUDE Q16b95No TotalOf[DummyQ16b95]

*QUESTION Q16b95 *FORM *IF [Q16bDummy=95]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.95}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.95}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA

*IF [ValueOf[Q16b95No]=2 & LengthOf[Q16b95.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b95No}"
*IF [ValueOf[Q16b95No]=3 & (LengthOf[Q16b95.2]<1 | LengthOf[Q16b95.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b95No}"


#### 96 
*QUESTION DummyQ16b96 *NUMLIST *DUMMY2
Number Of Card Q16b96
*USELIST "CardNList"

*IF [ValueOf[Q16a.1]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,1]
*IF [ValueOf[Q16a.2]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,2]
*IF [ValueOf[Q16a.3]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,3]
*IF [ValueOf[Q16a.4]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,4]
*IF [ValueOf[Q16a.5]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,5]
*IF [ValueOf[Q16a.6]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,6]
*IF [ValueOf[Q16a.7]=96] *INCLUDE DummyQ16b96 ValueInPositionOf[1,7]

*QUESTION Q16b96No *NUMBER *DUMMY2
Number Of Card Q16b96

*INCLUDE Q16b96No TotalOf[DummyQ16b96]

*QUESTION Q16b96 *FORM *IF [Q16bDummy=96]
Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
Tell me the full name of <font color="#FF73FF">{Q16bDummy.96}</font> credit card <font color="#FF73FF">[SA]</font>
<br><big>{Q16bDummy.96}</big>
1:Card name1 *ALPHA *MANDATORY
2:Card name2 *ALPHA
3:Card name3 *ALPHA

*IF [ValueOf[Q16b96No]=2 & LengthOf[Q16b96.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b96No}"
*IF [ValueOf[Q16b96No]=3 & (LengthOf[Q16b96.2]<1 | LengthOf[Q16b96.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b96No}"

#### 97 
#*QUESTION DummyQ16b97 *NUMLIST *DUMMY2
#Number Of Card Q16b97
#1:Card 1
#2:Card 2
#3:Card 3
#4:Card 4
#5:Card 5
#6:Card 6
#7:Card 7+
#
#*IF [ValueOf[Q16a.1]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,1]
#*IF [ValueOf[Q16a.2]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,2]
#*IF [ValueOf[Q16a.3]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,3]
#*IF [ValueOf[Q16a.4]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,4]
#*IF [ValueOf[Q16a.5]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,5]
#*IF [ValueOf[Q16a.6]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,6]
#*IF [ValueOf[Q16a.7]=97] *INCLUDE DummyQ16b97 ValueInPositionOf[1,7]
#
#*QUESTION Q16b97No *NUMBER *DUMMY2
#Number Of Card Q16b97
#
#*INCLUDE Q16b97No TotalOf[DummyQ16b97]
#
#
#
#*QUESTION Q16b97 *FORM *IF [Q16bDummy=97]
#Q16b. Now we request you to take out your <b>Credit card</b> and tell me the <b>exact name</b> of the credit card that you have?
#Tell me the full name of <font color="#FF73FF">{Q16bDummy.97}</font> credit card <font color="#FF73FF">[SA]</font>
#<br><big>{Q16bDummy.97}</big>
#1:Card name1 *ALPHA *MANDATORY
#2:Card name2 *ALPHA
#3:Card name3 *ALPHA
#
#
#
#*IF [ValueOf[Q16b97No]=2 & LengthOf[Q16b97.2]<1] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b97No}"
#*IF [ValueOf[Q16b97No]=3 & (LengthOf[Q16b97.2]<1 | LengthOf[Q16b97.3]<1)] *MSG "Number of Card Not Matched;Total Number of Card should be {Q16b97No}"

*LIST "CCList"
1:NEO Credit Card (Axis Bank)
2:AIRTEL AXIS BANK Credit Card (Axis Bank)
3:AXIS BANK ACE Credit Card (Axis Bank)
4:AXIS BANK ATLAS Credit Card (Axis Bank)
5:AXIS BANK AURA Credit Card (Axis Bank)
6:AXIS BANK FREECHARGE Credit Card (Axis Bank)
7:AXIS BANK FREECHARGE PLUS Credit Card (Axis Bank)
8:AXIS BANK MAGNUS Credit Card (Axis Bank)
9:AXIS BANK MY WINGS Credit Card (Axis Bank)
10:AXIS Bank MYZONE Credit Card (Axis Bank)
11:AXIS BANK MYZONE EASY Credit Card (Axis Bank)
12:AXIS BANK PRIDE PLATINUM Credit Card (Axis Bank)
13:AXIS BANK PRIDE SIGNATURE Credit Card (Axis Bank)
14:AXIS BANK PRIVILEGE Credit Card (Axis Bank)
15:AXIS BANK RESERVE Credit Card (Axis Bank)
16:AXIS BANK SELECT Credit Card (Axis Bank)
17:AXIS BANK SHOPPERS STOP Credit Card (Axis Bank)
18:AXIS BANK SIGNATURE CREDIT CARD WITH LIFESTYLE BENEFITS (Axis Bank)
19:AXIS BANK VISTARA Credit Card (Axis Bank)
20:AXIS BANK VISTARA INFINITE Credit Card (Axis Bank)
21:AXIS BANK VISTARA SIGNATURE  Credit Card (Axis Bank)
22:FIBE AXIS BANK Credit Card (Axis Bank)
23:FLIPKART AXIS BANK Credit Card (Axis Bank)
24:FLIPKART AXIS BANK SUPER ELITE Credit Card (Axis Bank)
25:INDIAN OIL AXIS BANK Credit Card (Axis Bank)
26:LIC AXIS BANK Credit Card (Axis Bank)
27:LIC AXIS BANK PLATINUM Credit Card (Axis Bank)
28:MILES AND MORE AXIS BANK Credit Card (Axis Bank)
29:PLATINUM Credit Card (Axis Bank)
30:PRIVILEGE EASY Credit Card (Axis Bank)
31:REWARDS Credit Card (Axis Bank)
32:SAMSUNG AXIS BANK INFINITE Credit Card (Axis Bank)
33:SAMSUNG AXIS BANK SIGNATURE Credit Card (Axis Bank)
34:SPICEJET AXIS BANK VOYAGE BLACK Credit Card (Axis Bank)
35:SPICEJET AXIS BANK VOYAGE Credit Card (Axis Bank)
36:TITANIUM SMART TRAVELLER  Credit Card (Axis Bank)
1198:{Q16b2.1198} (Axis Bank)
37:Aditya Birla SBI Card (SBI)
38:Aditya Birla SBI Card SELECT (SBI)
39:Air India SBI Platinum Card (SBI)
40:Apollo SBI Card (SBI)
41:BPCL SBI Card (SBI)
42:BPCL SBI Card OCTANE (SBI)
43:CASHBACK SBI Card (SBI)
44:Central SBI Select+ Card (SBI)
45:Club Vistara SBI Card (SBI)
46:Club Vistara SBI Card PRIME (SBI)
47:Doctor's SBI Card (SBI)
48:Doctor's SBI Card(in association with IMA) (SBI)
49:Etihad Guest SBI Card (SBI)
50:Etihad Guest SBI Premier Card (SBI)
51:FABINDIA SBI Card (SBI)
52:FABINDIA SBI Card SELECT (SBI)
53:IRCTC SBI CARD (on Rupay platform) (SBI)
54:IRCTC SBI Card Premier (SBI)
55:IRCTC SBI Platinum Card (SBI)
56:Lifestyle Home Centre SBI Card (SBI)
57:Lifestyle Home Centre SBI Card PRIME (SBI)
58:Lifestyle Home Centre SBI Card SELECT (SBI)
59:Max SBI Card (SBI)
60:Max SBI Card PRIME (SBI)
61:Max SBI Card SELECT (SBI)
62:Reliance SBI Card (SBI)
63:Reliance SBI Card PRIME (SBI)
64:SBI Card ELITE (SBI)
65:SBI Card Miles Elite (SBI)
66:SBI Card MILES (SBI)
67:SBI Card MILES PRIME (SBI)
68:SBI Card PRIME (SBI)
69:SBI Card PULSE (SBI)
70:SimplyCLICK SBI Card (SBI)
71:SimplySAVE UPI SBI Card (SBI)
72:Spar SBI Card (SBI)
73:Spar SBI Card PRIME (SBI)
74:Spar SBI Card SELECT (SBI)
75:Titan SBI Card Rupay (SBI)
76:Titan SBI Card Visa (SBI)
77:Yatra SBI Card (SBI)
298:{Q16b18.298} (SBI)
78:6E Rewards - IndiGo HDFC Bank Credit Card (HDFC)
79:6E Rewards XL- IndiGo HDFC Bank Credit Card (HDFC)
80:All Miles Credit Card (HDFC)
81:AP Accounts Payable Program (HDFC)
82:AR Accounts Receivable Program (HDFC)
83:Auto Insurance program (HDFC)
84:Best Price Save Max HDFC Bank Credit Card (HDFC)
85:Best Price Save Smart HDFC Bank Credit Card (HDFC)
86:Bharat Credit Card (HDFC)
87:Biz Black Metal Edition Credit Card (HDFC)
88:Biz First Credit Card (HDFC)
89:Biz Grow Credit Card (HDFC)
90:Biz Power Credit Card (HDFC)
91:Business Bharat Credit Card (HDFC)
92:Business Corporate MoneyBack Credit Card (HDFC)
93:Business Freedom Credit Card (HDFC)
94:Business Gold Credit Card (HDFC)
95:Business MoneyBack (HDFC)
96:Business Platinum Credit Card (HDFC)
97:Business Regalia (HDFC)
98:Business Regalia First Credit Card (HDFC)
99:Central Travel Account (HDFC)
100:Corporate Platinum Credit Card (HDFC)
101:Corporate Premium Credit Card (HDFC)
102:CSC Small Business MoneyBack (HDFC)
103:Dealer Credit Card (HDFC)
104:Diners Club Black Credit Card (HDFC)
105:Diners Club Black Metal Edition Credit Card (HDFC)
106:Diners Club Premium Credit Card (HDFC)
107:Diners Club Privilege Credit Card (HDFC)
108:Diners Club Rewardz Credit Card (HDFC)
109:Diners ClubMiles Credit Card (HDFC)
110:Doctor's Regalia Credit Card (HDFC)
111:Doctors Superia Credit Card (HDFC)
112:Easy EMI Card (HDFC)
113:Fleet Program (HDFC)
114:Flipkart Wholesale HDFC Bank Credit Card (HDFC)
115:Freedom Credit Card (HDFC)
116:HDFC Bank & SAP Concur Solutions Black Corporate Credit Card (HDFC)
117:HDFC Bank & SAP Concur Solutions Prime Corporate Credit Card (HDFC)
118:HDFC Bank Freedom RuPay Credit Card (HDFC)
119:HDFC Bank H.O.G Diners Club Credit Card (HDFC)
120:HDFC Bank Harley-Davidson Diners Club Credit Card  (HDFC)
121:HDFC Bank RuPay Shoppers Stop Credit Card (HDFC)
122:HDFC Bank Times Card Credit (HDFC)
123:HDFC Bank Times Card Credit Platinum (HDFC)
124:HDFC Bank UPI RuPay Biz Credit Card (HDFC)
125:HDFC BANK UPI RuPay Credit Card (HDFC)
126:IndianOil HDFC Bank Credit Card (HDFC)
127:INFINIA Metal Edition (HDFC)
128:InterMiles HDFC Bank Diners Club Credit Card (HDFC)
129:InterMiles HDFC Bank Platinum Credit Card (HDFC)
130:InterMiles HDFC Bank Signature Credit Card (HDFC)
131:IRCTC HDFC Bank Credit Card (HDFC)
132:JetPrivilege HDFC Bank Select / Titanium (HDFC)
133:Marriott Bonvoy HDFC Bank Credit Card (HDFC)
134:Millenia Credit Card (HDFC)
135:MoneyBack Credit Card (HDFC)
136:Moneyback+ Credit Card (HDFC)
137:Paytm HDFC Bank Business Credit Card (HDFC)
138:Paytm HDFC Bank Credit Card (HDFC)
139:Paytm HDFC Bank Digital Credit Card (HDFC)
140:Paytm HDFC Bank Mobile Credit Card (HDFC)
141:Paytm HDFC Bank SELECT Business Credit Card (HDFC)
142:Paytm HDFC Bank Select Credit Card (HDFC)
143:Pine Labs HDFC Bank Credit Card (HDFC)
144:Pine Labs HDFC Bank Pro Credit Card (HDFC)
145:Platinum Edge Credit Card (HDFC)
146:Platinum Plus Credit Card (HDFC)
147:Purchase Credit Card (HDFC)
148:Purchase MoneyBack Credit Card (HDFC)
149:Purchase Premium Credit card (HDFC)
150:Purchase Reward Card (HDFC)
151:Regalia Credit Card (HDFC)
152:Regalia First Credit Card (HDFC)
153:Regalia Gold Credit Card (HDFC)
154:Shoppers Stop Black HDFC Bank Credit Card (HDFC)
155:Shoppers Stop HDFC Bank Credit Card (HDFC)
156:Solitaire Credit Card (HDFC)
157:Superia Credit Card (HDFC)
158:Swiggy HDFC Bank Credit Card (HDFC)
159:Tata Neu Infinity HDFC Bank Credit Card (HDFC)
160:Tata Neu Plus HDFC Bank Credit Card (HDFC)
161:Teachers Platinum Credit Card (HDFC)
162:Titanium Edge Credit Card (HDFC)
163:TMC Card (HDFC)
164:Visa Signature Credit Card (HDFC)
165:World MasterCard Credit Card (HDFC)
398:{Q16b10.398}(HDFC)
166:Accelero (ICICI)
167:Amazon Pay ICICI Credit Card (ICICI)
168:Chennai Super Kings Credit Card (ICICI)
169:Coral Credit Card (ICICI)
170:Coral RuPay Credit Card (ICICI)
171:Emeralde Credit Card (ICICI)
172:Emeralde Private Metal Credit Card (ICICI)
173:Emirates (ICICI)
174:Expressions Card (ICICI)
175:HPCL Coral Visa Card (ICICI)
176:HPCL Super Saver Credit Card (ICICI)
177:ICICI Bank HPCL Super Saver RuPay Credit Card (ICICI)
178:MakeMyTrip ICICI Bank Platinum Credit Card (ICICI)
179:MakeMyTrip ICICI Bank Signature Credit Card (ICICI)
180:Manchester United Platinum Credit Card (ICICI)
181:Manchester United Signature Credit Card (ICICI)
182:Parakram Credit Card (ICICI)
183:Platinum Credit Card (ICICI)
184:Rubyx Credit Card (ICICI)
185:Sapphiro Credit Card (ICICI)
498:{Q16b12.498}(ICICI)
186:811 #DreamDifferent Credit Card (Kotak)
187:Best Price Credit Card (Kotak)
188:Biz Credit Card (Kotak)
189:Corporate Gold Credit Card (Kotak)
190:Corporate Platinum Credit Card (Kotak)
191:Corporate Wealth Signature Credit Card (Kotak)
192:IndianOil Kotak Credit Card (Kotak)
193:Indigo 6E Rewards Credit Card (Kotak)
194:Kotak 811 Credit Card (Kotak)
195:Kotak Indigo 6E Reqards XL Credit Card (Kotak)
196:Kotak Infinite Credit Card (Kotak)
197:Kotak UPI RuPay Credit Card (Kotak)
198:League Platinum Credit Card (Kotak)
199:Metro Kotak Credit Card (Kotak)
200:Mojo Platinum Credit Card (Kotak)
201:Myntra Kotak Credit Card (Kotak)
202:NRI Royale Signature Credit Card (Kotak)
203:Privy League Signature Credit Card (Kotak)
204:PVR Inox Kotak Credit Card (Kotak)
205:PVR Kotak Gold Credit Card (Kotak)
206:PVR Kotak Platinum Credit Card (Kotak)
207:Royal Signature Credit Card (Kotak)
208:Urbane Gold Credit Card (Kotak)
209:White Credit Card (Kotak)
210:White Reserve Credit Card (Kotak)
211:Zen Signature Credit Card (Kotak)
598:{Q16b15.598} (Kotak)
212:HSBC Cashback Credit Card (HSBC)
213:HSBC Premier Credit Card (HSBC)
214:HSBC Visa Platinum Credit Card (HSBC)
698:{Q16b11.698} (HSBC)
215:American Express Gold Card (American Express)
216:American Express Membership Rewards Credit Card (American Express)
217:American Express Platinum Card (American Express)
218:American Express Platinum Reserve Credit Card (American Express)
219:American Express Platinum Travel Credit Card (American Express)
220:American Express SmartEarn Credit Card (American Express)
798:{Q16b1.798} (American Express)
221:Citi Cash Back Credit Card (Citi)
222:Citi PremierMiles Credit Card (Citi)
223:Citi Prestige Credit Card (Citi)
224:Citi Rewards Credit Card (Citi)
225:First Citizen Citi Credit Card (Citi)
226:IKEA Family Credit Card by Citi (Citi)
227:IndianOil Citi Credit Card (Citi)
228:Paytm Credit Card (Citi)
898:{Q16b8.898}(Citi)
301:{Q16b3.1} (Bandhan Bank)
302:{Q16b3.2} (Bandhan Bank)
303:{Q16b3.3} (Bandhan Bank)
311:{Q16b4.1} (Bank of Baroda)
312:{Q16b4.2} (Bank of Baroda)
313:{Q16b4.3} (Bank of Baroda)
321:{Q16b5.1} (Bank of India)
322:{Q16b5.2} (Bank of India)
323:{Q16b5.3} (Bank of India)
331:{Q16b6.1} (Bank of Maharashtra)
332:{Q16b6.2} (Bank of Maharashtra)
333:{Q16b6.3} (Bank of Maharashtra)
341:{Q16b7.1} (Canara Bank)
342:{Q16b7.2} (Canara Bank)
343:{Q16b7.3} (Canara Bank)
351:{Q16b9.1} (DBS)
352:{Q16b9.2} (DBS)
353:{Q16b9.3} (DBS)
361:{Q16b13.1} (IDFC First Bank)
362:{Q16b13.2} (IDFC First Bank)
363:{Q16b13.3} (IDFC First Bank)
371:{Q16b14.1} (IndusInd Bank)
372:{Q16b14.2} (IndusInd Bank)
373:{Q16b14.3} (IndusInd Bank)
381:{Q16b16.1} (Punjab National Bank)
382:{Q16b16.2} (Punjab National Bank)
383:{Q16b16.3} (Punjab National Bank)
391:{Q16b17.1} (Standard Chartered Bank)
392:{Q16b17.2} (Standard Chartered Bank)
393:{Q16b17.3} (Standard Chartered Bank)
401:{Q16b19.1} (YES Bank)
402:{Q16b19.2} (YES Bank)
403:{Q16b19.3} (YES Bank)
411:{Q16b95.1} ({Q13a.95})
412:{Q16b95.2} ({Q13a.95})
413:{Q16b95.3} ({Q13a.95})
421:{Q16b96.1} ({Q13b.96})
422:{Q16b96.2} ({Q13b.96})
423:{Q16b96.3} ({Q13b.96})
#431:{Q16b97.1} ({Q13b.97})
#432:{Q16b97.2} ({Q13b.97})
#433:{Q16b97.3} ({Q13b.97})

*LIST "CCListImage"
1:NEO Credit Card   *PICT "001Axis.jpg"
2:AIRTEL AXIS BANK Credit Card   *PICT "002Axis.jpg"
3:AXIS BANK ACE Credit Card   *PICT "003Axis.jpg"
4:AXIS BANK ATLAS Credit Card   *PICT "004Axis.jpg"
5:AXIS BANK AURA Credit Card   *PICT "005Axis.jpg"
6:AXIS BANK FREECHARGE Credit Card   *PICT "006Axis.jpg"
7:AXIS BANK FREECHARGE PLUS Credit Card   *PICT "007Axis.jpg"
8:AXIS BANK MAGNUS Credit Card   *PICT "008Axis.jpg"
9:AXIS BANK MY WINGS Credit Card   *PICT "009Axis.jpg"
10:AXIS Bank MYZONE Credit Card   *PICT "010Axis.jpg"
11:AXIS BANK MYZONE EASY Credit Card   *PICT "011Axis.jpg"
12:AXIS BANK PRIDE PLATINUM Credit Card   *PICT "012Axis.jpg"
13:AXIS BANK PRIDE SIGNATURE Credit Card   *PICT "013Axis.jpg"
14:AXIS BANK PRIVILEGE Credit Card   *PICT "014Axis.jpg"
15:AXIS BANK RESERVE Credit Card   *PICT "015Axis.jpg"
16:AXIS BANK SELECT Credit Card   *PICT "016Axis.jpg"
17:AXIS BANK SHOPPERS STOP Credit Card   *PICT "017Axis.jpg"
18:AXIS BANK SIGNATURE CREDIT CARD WITH LIFESTYLE BENEFITS   *PICT "018Axis.jpg"
19:AXIS BANK VISTARA Credit Card   *PICT "019Axis.jpg"
20:AXIS BANK VISTARA INFINITE Credit Card   *PICT "020Axis.jpg"
21:AXIS BANK VISTARA SIGNATURE  Credit Card   *PICT "021Axis.jpg"
22:FIBE AXIS BANK Credit Card   *PICT "022Axis.jpg"
23:FLIPKART AXIS BANK Credit Card   *PICT "023Axis.jpg"
24:FLIPKART AXIS BANK SUPER ELITE Credit Card   *PICT "024Axis.jpg"
25:INDIAN OIL AXIS BANK Credit Card   *PICT "025Axis.jpg"
26:LIC AXIS BANK Credit Card   *PICT "026Axis.jpg"
27:LIC AXIS BANK PLATINUM Credit Card   *PICT "027Axis.jpg"
28:MILES AND MORE AXIS BANK Credit Card   *PICT "028Axis.jpg"
29:PLATINUM Credit Card (Axis)   *PICT "029Axis.jpg"
30:PRIVILEGE EASY Credit Card   *PICT "030Axis.jpg"
31:REWARDS Credit Card   *PICT "031Axis.jpg"
32:SAMSUNG AXIS BANK INFINITE Credit Card   *PICT "032Axis.jpg"
33:SAMSUNG AXIS BANK SIGNATURE Credit Card   *PICT "033Axis.jpg"
34:SPICEJET AXIS BANK VOYAGE BLACK Credit Card   *PICT "034Axis.jpg"
35:SPICEJET AXIS BANK VOYAGE Credit Card   *PICT "035Axis.jpg"
36:TITANIUM SMART TRAVELLER  Credit Card   *PICT "036Axis.jpg"
1198:{Q16b2.1198} (Axis Bank) *PICT "OtherCards.jpg"

37:Aditya Birla SBI Card   *PICT "037SBI.jpg"
38:Aditya Birla SBI Card SELECT   *PICT "038SBI.jpg"
39:Air India SBI Platinum Card   *PICT "039SBI.jpg"
40:Apollo SBI Card   *PICT "040SBI.jpg"
41:BPCL SBI Card   *PICT "041SBI.jpg"
42:BPCL SBI Card OCTANE   *PICT "042SBI.jpg"
43:CASHBACK SBI Card   *PICT "043SBI.jpg"
44:Central SBI Select+ Card   *PICT "044SBI.jpg"
45:Club Vistara SBI Card   *PICT "045SBI.jpg"
46:Club Vistara SBI Card PRIME   *PICT "046SBI.jpg"
47:Doctor's SBI Card   *PICT "047SBI.jpg"
48:Doctor's SBI Card(in association with IMA)   *PICT "048SBI.jpg"
49:Etihad Guest SBI Card   *PICT "049SBI.jpg"
50:Etihad Guest SBI Premier Card   *PICT "050SBI.jpg"
51:FABINDIA SBI Card   *PICT "051SBI.jpg"
52:FABINDIA SBI Card SELECT   *PICT "052SBI.jpg"
53:IRCTC SBI CARD (on Rupay platform)   *PICT "053SBI.jpg"
54:IRCTC SBI Card Premier   *PICT "054SBI.jpg"
55:IRCTC SBI Platinum Card   *PICT "055SBI.jpg"
56:Lifestyle Home Centre SBI Card   *PICT "056SBI.jpg"
57:Lifestyle Home Centre SBI Card PRIME   *PICT "057SBI.jpg"
58:Lifestyle Home Centre SBI Card SELECT   *PICT "058SBI.jpg"
59:Max SBI Card   *PICT "059SBI.jpg"
60:Max SBI Card PRIME   *PICT "060SBI.jpg"
61:Max SBI Card SELECT   *PICT "061SBI.jpg"
62:Reliance SBI Card   *PICT "062SBI.jpg"
63:Reliance SBI Card PRIME   *PICT "063SBI.jpg"
64:SBI Card ELITE   *PICT "064SBI.jpg"
65:SBI Card Miles Elite   *PICT "065SBI.jpg"
66:SBI Card MILES   *PICT "066SBI.jpg"
67:SBI Card MILES PRIME   *PICT "067SBI.jpg"
68:SBI Card PRIME   *PICT "068SBI.jpg"
69:SBI Card PULSE   *PICT "069SBI.jpg"
70:SimplyCLICK SBI Card   *PICT "070SBI.jpg"
71:SimplySAVE UPI SBI Card   *PICT "071SBI.jpg"
72:Spar SBI Card   *PICT "072SBI.jpg"
73:Spar SBI Card PRIME   *PICT "073SBI.jpg"
74:Spar SBI Card SELECT   *PICT "074SBI.jpg"
75:Titan SBI Card Rupay   *PICT "075SBI.jpg"
76:Titan SBI Card Visa   *PICT "076SBI.jpg"
77:Yatra SBI Card   *PICT "077SBI.jpg"
298:{Q16b18.298} (SBI) *PICT "OtherCards.jpg"

78:6E Rewards - IndiGo HDFC Bank Credit Card   *PICT "078Hdfc.jpg"
79:6E Rewards XL- IndiGo HDFC Bank Credit Card   *PICT "079Hdfc.jpg"
80:All Miles Credit Card   *PICT "080Hdfc.jpg"
81:AP Accounts Payable Program   *PICT "081Hdfc.jpg"
82:AR Accounts Receivable Program   *PICT "082Hdfc.jpg"
83:Auto Insurance program   *PICT "083Hdfc.jpg"
84:Best Price Save Max HDFC Bank Credit Card   *PICT "084Hdfc.jpg"
85:Best Price Save Smart HDFC Bank Credit Card   *PICT "085Hdfc.jpg"
86:Bharat Credit Card   *PICT "086Hdfc.jpg"
87:Biz Black Metal Edition Credit Card   *PICT "087Hdfc.jpg"
88:Biz First Credit Card   *PICT "088Hdfc.jpg"
89:Biz Grow Credit Card   *PICT "089Hdfc.jpg"
90:Biz Power Credit Card   *PICT "090Hdfc.jpg"
91:Business Bharat Credit Card   *PICT "091Hdfc.jpg"
92:Business Corporate MoneyBack Credit Card   *PICT "092Hdfc.jpg"
93:Business Freedom Credit Card   *PICT "093Hdfc.jpg"
94:Business Gold Credit Card   *PICT "094Hdfc.jpg"
95:Business MoneyBack   *PICT "095Hdfc.jpg"
96:Business Platinum Credit Card   *PICT "096Hdfc.jpg"
97:Business Regalia   *PICT "097Hdfc.jpg"
98:Business Regalia First Credit Card   *PICT "098Hdfc.jpg"
99:Central Travel Account   *PICT "099Hdfc.jpg"
100:Corporate Platinum Credit Card (HDFC)  *PICT "100Hdfc.jpg"
101:Corporate Premium Credit Card   *PICT "101Hdfc.jpg"
102:CSC Small Business MoneyBack   *PICT "102Hdfc.jpg"
103:Dealer Credit Card   *PICT "103Hdfc.jpg"
104:Diners Club Black Credit Card   *PICT "104Hdfc.jpg"
105:Diners Club Black Metal Edition Credit Card   *PICT "105Hdfc.jpg"
106:Diners Club Premium Credit Card   *PICT "106Hdfc.jpg"
107:Diners Club Privilege Credit Card   *PICT "107Hdfc.jpg"
108:Diners Club Rewardz Credit Card   *PICT "108Hdfc.jpg"
109:Diners ClubMiles Credit Card   *PICT "109Hdfc.jpg"
110:Doctor's Regalia Credit Card   *PICT "110Hdfc.jpg"
111:Doctors Superia Credit Card   *PICT "111Hdfc.jpg"
112:Easy EMI Card   *PICT "112Hdfc.jpg"
113:Fleet Program   *PICT "113Hdfc.jpg"
114:Flipkart Wholesale HDFC Bank Credit Card   *PICT "114Hdfc.jpg"
115:Freedom Credit Card   *PICT "115Hdfc.jpg"
116:HDFC Bank & SAP Concur Solutions Black Corporate Credit Card   *PICT "116Hdfc.jpg"
117:HDFC Bank & SAP Concur Solutions Prime Corporate Credit Card   *PICT "117Hdfc.jpg"
118:HDFC Bank Freedom RuPay Credit Card   *PICT "118Hdfc.jpg"
119:HDFC Bank H.O.G Diners Club Credit Card   *PICT "119Hdfc.jpg"
120:HDFC Bank Harley-Davidson Diners Club Credit Card    *PICT "120Hdfc.jpg"
121:HDFC Bank RuPay Shoppers Stop Credit Card   *PICT "121Hdfc.jpg"
122:HDFC Bank Times Card Credit   *PICT "122Hdfc.jpg"
123:HDFC Bank Times Card Credit Platinum   *PICT "123Hdfc.jpg"
124:HDFC Bank UPI RuPay Biz Credit Card   *PICT "124Hdfc.jpg"
125:HDFC BANK UPI RuPay Credit Card   *PICT "125Hdfc.jpg"
126:IndianOil HDFC Bank Credit Card   *PICT "126Hdfc.jpg"
127:INFINIA Metal Edition   *PICT "127Hdfc.jpg"
128:InterMiles HDFC Bank Diners Club Credit Card   *PICT "128Hdfc.jpg"
129:InterMiles HDFC Bank Platinum Credit Card   *PICT "129Hdfc.jpg"
130:InterMiles HDFC Bank Signature Credit Card   *PICT "130Hdfc.jpg"
131:IRCTC HDFC Bank Credit Card   *PICT "131Hdfc.jpg"
132:JetPrivilege HDFC Bank Select / Titanium   *PICT "132Hdfc.jpg"
133:Marriott Bonvoy HDFC Bank Credit Card   *PICT "133Hdfc.jpg"
134:Millenia Credit Card   *PICT "134Hdfc.jpg"
135:MoneyBack Credit Card   *PICT "135Hdfc.jpg"
136:Moneyback+ Credit Card   *PICT "136Hdfc.jpg"
137:Paytm HDFC Bank Business Credit Card   *PICT "137Hdfc.jpg"
138:Paytm HDFC Bank Credit Card   *PICT "138Hdfc.jpg"
139:Paytm HDFC Bank Digital Credit Card   *PICT "139Hdfc.jpg"
140:Paytm HDFC Bank Mobile Credit Card   *PICT "140Hdfc.jpg"
141:Paytm HDFC Bank SELECT Business Credit Card   *PICT "141Hdfc.jpg"
142:Paytm HDFC Bank Select Credit Card   *PICT "142Hdfc.jpg"
143:Pine Labs HDFC Bank Credit Card   *PICT "143Hdfc.jpg"
144:Pine Labs HDFC Bank Pro Credit Card   *PICT "144Hdfc.jpg"
145:Platinum Edge Credit Card   *PICT "145Hdfc.jpg"
146:Platinum Plus Credit Card   *PICT "146Hdfc.jpg"
147:Purchase Credit Card   *PICT "147Hdfc.jpg"
148:Purchase MoneyBack Credit Card   *PICT "148Hdfc.jpg"
149:Purchase Premium Credit card   *PICT "149Hdfc.jpg"
150:Purchase Reward Card   *PICT "150Hdfc.jpg"
151:Regalia Credit Card   *PICT "151Hdfc.jpg"
152:Regalia First Credit Card   *PICT "152Hdfc.jpg"
153:Regalia Gold Credit Card   *PICT "153Hdfc.jpg"
154:Shoppers Stop Black HDFC Bank Credit Card   *PICT "154Hdfc.jpg"
155:Shoppers Stop HDFC Bank Credit Card   *PICT "155Hdfc.jpg"
156:Solitaire Credit Card   *PICT "156Hdfc.jpg"
157:Superia Credit Card   *PICT "157Hdfc.jpg"
158:Swiggy HDFC Bank Credit Card   *PICT "158Hdfc.jpg"
159:Tata Neu Infinity HDFC Bank Credit Card   *PICT "159Hdfc.jpg"
160:Tata Neu Plus HDFC Bank Credit Card   *PICT "160Hdfc.jpg"
161:Teachers Platinum Credit Card   *PICT "161Hdfc.jpg"
162:Titanium Edge Credit Card   *PICT "162Hdfc.jpg"
163:TMC Card   *PICT "163Hdfc.jpg"
164:Visa Signature Credit Card   *PICT "164Hdfc.jpg"
165:World MasterCard Credit Card   *PICT "165Hdfc.jpg"
398:{Q16b10.398}(HDFC) *PICT "OtherCards.jpg"


166:Accelero   *PICT "166ICICI.jpg"
167:Amazon Pay ICICI Credit Card   *PICT "167ICICI.jpg"
168:Chennai Super Kings Credit Card   *PICT "168ICICI.jpg"
169:Coral Credit Card   *PICT "169ICICI.jpg"
170:Coral RuPay Credit Card   *PICT "170ICICI.jpg"
171:Emeralde Credit Card   *PICT "171ICICI.jpg"
172:Emeralde Private Metal Credit Card   *PICT "172ICICI.jpg"
173:Emirates   *PICT "173ICICI.jpg"
174:Expressions Card   *PICT "174ICICI.jpg"
175:HPCL Coral Visa Card   *PICT "175ICICI.jpg"
176:HPCL Super Saver Credit Card   *PICT "176ICICI.jpg"
177:ICICI Bank HPCL Super Saver RuPay Credit Card   *PICT "177ICICI.jpg"
178:MakeMyTrip ICICI Bank Platinum Credit Card   *PICT "178ICICI.jpg"
179:MakeMyTrip ICICI Bank Signature Credit Card   *PICT "179ICICI.jpg"
180:Manchester United Platinum Credit Card   *PICT "180ICICI.jpg"
181:Manchester United Signature Credit Card   *PICT "181ICICI.jpg"
182:Parakram Credit Card   *PICT "182ICICI.jpg"
183:Platinum Credit Card   *PICT "183ICICI.jpg"
184:Rubyx Credit Card   *PICT "184ICICI.jpg"
185:Sapphiro Credit Card   *PICT "185ICICI.jpg"
498:{Q16b12.498}(ICICI) *PICT "OtherCards.jpg"


186:811 #DreamDifferent Credit Card   *PICT "186Kotak.jpg"
187:Best Price Credit Card   *PICT "187Kotak.jpg"
188:Biz Credit Card   *PICT "188Kotak.jpg"
189:Corporate Gold Credit Card   *PICT "189Kotak.jpg"
190:Corporate Platinum Credit Card   *PICT "190Kotak.jpg"
191:Corporate Wealth Signature Credit Card   *PICT "191Kotak.jpg"
192:IndianOil Kotak Credit Card   *PICT "192Kotak.jpg"
193:Indigo 6E Rewards Credit Card   *PICT "193Kotak.jpg"
194:Kotak 811 Credit Card   *PICT "194Kotak.jpg"
195:Kotak Indigo 6E Reqards XL Credit Card   *PICT "195Kotak.jpg"
196:Kotak Infinite Credit Card   *PICT "196Kotak.jpg"
197:Kotak UPI RuPay Credit Card   *PICT "197Kotak.jpg"
198:League Platinum Credit Card   *PICT "198Kotak.jpg"
199:Metro Kotak Credit Card   *PICT "199Kotak.jpg"
200:Mojo Platinum Credit Card   *PICT "200Kotak.jpg"
201:Myntra Kotak Credit Card   *PICT "201Kotak.jpg"
202:NRI Royale Signature Credit Card   *PICT "202Kotak.jpg"
203:Privy League Signature Credit Card   *PICT "203Kotak.jpg"
204:PVR Inox Kotak Credit Card   *PICT "204Kotak.jpg"
205:PVR Kotak Gold Credit Card   *PICT "205Kotak.jpg"
206:PVR Kotak Platinum Credit Card   *PICT "206Kotak.jpg"
207:Royal Signature Credit Card   *PICT "207Kotak.jpg"
208:Urbane Gold Credit Card   *PICT "208Kotak.jpg"
209:White Credit Card   *PICT "209Kotak.jpg"
210:White Reserve Credit Card   *PICT "210Kotak.jpg"
211:Zen Signature Credit Card   *PICT "211Kotak.jpg"
598:{Q16b15.598} (Kotak) *PICT "OtherCards.jpg"

212:HSBC Cashback Credit Card   *PICT "212HSBC.jpg"
213:HSBC Premier Credit Card   *PICT "213HSBC.jpg"
214:HSBC Visa Platinum Credit Card   *PICT "214HSBC.jpg"
698:{Q16b11.698} (HSBC) *PICT "OtherCards.jpg"

215:American Express Gold Card   *PICT "215AEx.jpg"
216:American Express Membership Rewards Credit Card   *PICT "216AEx.jpg"
217:American Express Platinum Card   *PICT "217AEx.jpg"
218:American Express Platinum Reserve Credit Card   *PICT "218AEx.jpg"
219:American Express Platinum Travel Credit Card   *PICT "219AEx.jpg"
220:American Express SmartEarn Credit Card   *PICT "220AEx.jpg"
798:{Q16b1.798} (American Express) *PICT "OtherCards.jpg"

221:Citi Cash Back Credit Card   *PICT "221Citi.jpg"
222:Citi PremierMiles Credit Card   *PICT "222Citi.jpg"
223:Citi Prestige Credit Card   *PICT "223Citi.jpg"
224:Citi Rewards Credit Card   *PICT "224Citi.jpg"
225:First Citizen Citi Credit Card   *PICT "225Citi.jpg"
226:IKEA Family Credit Card by Citi   *PICT "226Citi.jpg"
227:IndianOil Citi Credit Card   *PICT "227Citi.jpg"
228:Paytm Credit Card   *PICT "228Citi.jpg"
898:{Q16b8.898}(Citi) *PICT "OtherCards.jpg"

301:{Q16b3.1} (Bandhan Bank) *PICT "OtherCards.jpg"
302:{Q16b3.2} (Bandhan Bank) *PICT "OtherCards.jpg"
303:{Q16b3.3} (Bandhan Bank) *PICT "OtherCards.jpg"
311:{Q16b4.1} (Bank of Baroda) *PICT "OtherCards.jpg"
312:{Q16b4.2} (Bank of Baroda) *PICT "OtherCards.jpg"
313:{Q16b4.3} (Bank of Baroda) *PICT "OtherCards.jpg"
321:{Q16b5.1} (Bank of India) *PICT "OtherCards.jpg"
322:{Q16b5.2} (Bank of India) *PICT "OtherCards.jpg"
323:{Q16b5.3} (Bank of India) *PICT "OtherCards.jpg"
331:{Q16b6.1} (Bank of Maharashtra) *PICT "OtherCards.jpg"
332:{Q16b6.2} (Bank of Maharashtra) *PICT "OtherCards.jpg"
333:{Q16b6.3} (Bank of Maharashtra) *PICT "OtherCards.jpg"
341:{Q16b7.1} (Canara Bank) *PICT "OtherCards.jpg"
342:{Q16b7.2} (Canara Bank) *PICT "OtherCards.jpg"
343:{Q16b7.3} (Canara Bank) *PICT "OtherCards.jpg"
351:{Q16b9.1} (DBS) *PICT "OtherCards.jpg"
352:{Q16b9.2} (DBS) *PICT "OtherCards.jpg"
353:{Q16b9.3} (DBS) *PICT "OtherCards.jpg"
361:{Q16b13.1} (IDFC First Bank) *PICT "OtherCards.jpg"
362:{Q16b13.2} (IDFC First Bank) *PICT "OtherCards.jpg"
363:{Q16b13.3} (IDFC First Bank) *PICT "OtherCards.jpg"
371:{Q16b14.1} (IndusInd Bank) *PICT "OtherCards.jpg"
372:{Q16b14.2} (IndusInd Bank) *PICT "OtherCards.jpg"
373:{Q16b14.3} (IndusInd Bank) *PICT "OtherCards.jpg"
381:{Q16b16.1} (Punjab National Bank) *PICT "OtherCards.jpg"
382:{Q16b16.2} (Punjab National Bank) *PICT "OtherCards.jpg"
383:{Q16b16.3} (Punjab National Bank) *PICT "OtherCards.jpg"
391:{Q16b17.1} (Standard Chartered Bank) *PICT "OtherCards.jpg"
392:{Q16b17.2} (Standard Chartered Bank) *PICT "OtherCards.jpg"
393:{Q16b17.3} (Standard Chartered Bank) *PICT "OtherCards.jpg"
401:{Q16b19.1} (YES Bank) *PICT "OtherCards.jpg"
402:{Q16b19.2} (YES Bank) *PICT "OtherCards.jpg"
403:{Q16b19.3} (YES Bank) *PICT "OtherCards.jpg"
411:{Q16b95.1} ({Q13a.95}) *PICT "OtherCards.jpg"
412:{Q16b95.2} ({Q13a.95}) *PICT "OtherCards.jpg"
413:{Q16b95.3} ({Q13a.95}) *PICT "OtherCards.jpg"
421:{Q16b96.1} ({Q13b.96}) *PICT "OtherCards.jpg"
422:{Q16b96.2} ({Q13b.96}) *PICT "OtherCards.jpg"
423:{Q16b96.3} ({Q13b.96}) *PICT "OtherCards.jpg"
#431:{Q16b97.1} ({Q13b.97}) *PICT "OtherCards.jpg"
#432:{Q16b97.2} ({Q13b.97}) *PICT "OtherCards.jpg"
#433:{Q16b97.3} ({Q13b.97}) *PICT "OtherCards.jpg"

*QUESTION CardBrand  *MR *DUMMY2
Card Brand Selection
*USELIST "CCList"

*INCLUDE CardBrand Q16b1
*INCLUDE CardBrand Q16b2
*INCLUDE CardBrand Q16b8
*INCLUDE CardBrand Q16b10
*INCLUDE CardBrand Q16b11
*INCLUDE CardBrand Q16b12
*INCLUDE CardBrand Q16b15
*INCLUDE CardBrand Q16b18

*IF [LengthOf[Q16b3.1]>0] *INCLUDE CardBrand [301]
*IF [LengthOf[Q16b3.2]>0] *INCLUDE CardBrand [302]
*IF [LengthOf[Q16b3.3]>0] *INCLUDE CardBrand [303]

*IF [LengthOf[Q16b4.1]>0] *INCLUDE CardBrand [311]
*IF [LengthOf[Q16b4.2]>0] *INCLUDE CardBrand [312]
*IF [LengthOf[Q16b4.3]>0] *INCLUDE CardBrand [313]

*IF [LengthOf[Q16b5.1]>0] *INCLUDE CardBrand [321]
*IF [LengthOf[Q16b5.2]>0] *INCLUDE CardBrand [322]
*IF [LengthOf[Q16b5.3]>0] *INCLUDE CardBrand [323]

*IF [LengthOf[Q16b6.1]>0] *INCLUDE CardBrand [331]
*IF [LengthOf[Q16b6.2]>0] *INCLUDE CardBrand [332]
*IF [LengthOf[Q16b6.3]>0] *INCLUDE CardBrand [333]

*IF [LengthOf[Q16b7.1]>0] *INCLUDE CardBrand [341]
*IF [LengthOf[Q16b7.2]>0] *INCLUDE CardBrand [342]
*IF [LengthOf[Q16b7.3]>0] *INCLUDE CardBrand [343]

*IF [LengthOf[Q16b9.1]>0] *INCLUDE CardBrand [351]
*IF [LengthOf[Q16b9.2]>0] *INCLUDE CardBrand [352]
*IF [LengthOf[Q16b9.3]>0] *INCLUDE CardBrand [353]

*IF [LengthOf[Q16b13.1]>0] *INCLUDE CardBrand [361]
*IF [LengthOf[Q16b13.2]>0] *INCLUDE CardBrand [362]
*IF [LengthOf[Q16b13.3]>0] *INCLUDE CardBrand [363]

*IF [LengthOf[Q16b14.1]>0] *INCLUDE CardBrand [371]
*IF [LengthOf[Q16b14.2]>0] *INCLUDE CardBrand [372]
*IF [LengthOf[Q16b14.3]>0] *INCLUDE CardBrand [373]

*IF [LengthOf[Q16b16.1]>0] *INCLUDE CardBrand [381]
*IF [LengthOf[Q16b16.2]>0] *INCLUDE CardBrand [382]
*IF [LengthOf[Q16b16.3]>0] *INCLUDE CardBrand [383]

*IF [LengthOf[Q16b17.1]>0] *INCLUDE CardBrand [391]
*IF [LengthOf[Q16b17.2]>0] *INCLUDE CardBrand [392]
*IF [LengthOf[Q16b17.3]>0] *INCLUDE CardBrand [393]

*IF [LengthOf[Q16b19.1]>0] *INCLUDE CardBrand [401]
*IF [LengthOf[Q16b19.2]>0] *INCLUDE CardBrand [402]
*IF [LengthOf[Q16b19.3]>0] *INCLUDE CardBrand [403]

*IF [LengthOf[Q16b95.1]>0] *INCLUDE CardBrand [411]
*IF [LengthOf[Q16b95.2]>0] *INCLUDE CardBrand [412]
*IF [LengthOf[Q16b95.3]>0] *INCLUDE CardBrand [413]

*IF [LengthOf[Q16b96.1]>0] *INCLUDE CardBrand [421]
*IF [LengthOf[Q16b96.2]>0] *INCLUDE CardBrand [422]
*IF [LengthOf[Q16b96.3]>0] *INCLUDE CardBrand [423]

#*IF [LengthOf[Q16b97.1]>0] *INCLUDE CardBrand [431]
#*IF [LengthOf[Q16b97.2]>0] *INCLUDE CardBrand [432]
#*IF [LengthOf[Q16b97.3]>0] *INCLUDE CardBrand [433]

*ENDREC

###########################################################################
###########################################################################

*QUESTION Q16c *NUMLISTTOTAL *INCLUDE [CardBrand] *IF [NumberOfResponse[CardBrand]>1]
Q16c. Now think of all the <b>payments made through credit cards in last 3 months</b> by you. Please tell in terms of % <b>how would this get divided amongst the various credit cards</b> that you have? Meaning what % got paid through your card one, similarly what % through 2nd card & so on…. 
Please consider only the payments made through credit cards in last 3 months & not any other mode of payments. 
*USELIST "CCList"
998:Total *NOCON *NMUL  *MIN 100 *MAX 100
#*MIN 100 *MAX 100 *MANDATORY

*IF [ValueOf[Q16c.998]!=100] *MSG "Invalid Response. Total should be 100"

*GRIDLIST "GQ16d"
1:Less than 1 year
2:1-3 years
3:3-5 years
4:5-7 years
5:7-10 years
6:10-12 years
7:12-15 years
8:More than 15  years


*QUESTION Q16d *GRIDSR *USEGRIDLIST "GQ16d" *INCLUDE [CardBrand] *IF [NumberOfResponse[CardBrand]>1]
Q16d. Since when have you been using  ..... ?  (SA per row) 
*USELIST "CCListImage"

*QUESTION Q16eChk *MR *DUMMY2
Q16e Check
*USELIST "YN"

*INCLUDE Q16eChk [2]

IF [ValueOf[Q16d.1]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.2]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.3]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.4]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.5]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.6]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.7]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.8]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.9]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.10]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.11]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.12]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.13]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.14]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.15]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.16]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.17]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.18]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.19]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.20]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.21]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.22]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.23]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.24]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.25]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.26]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.27]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.28]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.29]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.30]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.31]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.32]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.33]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.34]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.35]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.36]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.1198]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.37]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.38]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.39]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.40]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.41]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.42]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.43]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.44]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.45]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.46]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.47]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.48]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.49]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.50]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.51]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.52]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.53]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.54]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.55]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.56]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.57]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.58]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.59]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.60]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.61]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.62]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.63]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.64]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.65]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.66]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.67]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.68]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.69]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.70]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.71]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.72]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.73]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.74]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.75]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.76]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.77]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.29]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.78]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.79]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.80]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.81]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.82]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.83]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.84]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.85]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.86]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.87]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.88]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.89]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.90]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.91]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.92]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.93]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.94]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.95]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.96]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.97]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.98]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.99]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.100]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.101]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.102]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.103]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.104]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.105]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.106]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.107]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.108]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.109]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.110]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.111]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.112]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.113]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.114]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.115]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.116]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.117]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.118]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.119]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.120]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.121]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.122]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.123]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.124]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.125]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.126]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.127]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.128]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.129]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.130]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.131]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.132]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.133]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.134]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.135]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.136]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.137]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.138]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.139]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.140]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.141]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.142]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.143]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.144]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.145]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.146]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.147]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.148]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.149]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.150]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.151]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.152]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.153]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.154]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.155]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.156]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.157]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.158]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.159]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.160]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.161]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.162]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.163]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.164]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.165]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.398]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.166]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.167]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.168]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.169]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.170]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.171]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.172]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.173]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.174]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.175]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.176]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.177]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.178]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.179]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.180]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.181]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.182]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.183]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.184]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.185]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.498]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.186]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.187]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.188]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.189]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.190]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.191]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.192]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.193]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.194]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.195]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.196]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.197]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.198]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.199]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.200]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.201]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.202]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.203]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.204]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.205]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.206]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.207]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.208]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.209]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.210]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.211]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.598]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.212]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.213]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.214]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.698]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.215]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.216]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.217]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.218]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.219]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.220]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.798]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.221]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.222]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.223]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.224]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.225]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.226]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.227]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.228]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.898]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.301]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.302]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.303]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.311]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.312]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.313]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.321]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.322]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.323]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.331]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.332]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.333]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.341]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.342]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.343]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.351]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.352]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.353]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.361]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.362]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.363]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.371]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.372]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.373]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.381]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.382]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.383]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.391]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.392]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.393]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.401]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.402]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.403]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.411]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.412]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.413]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.421]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.422]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.423]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.431]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.432]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]
IF [ValueOf[Q16d.433]>ValueOf[Q12e]] *INCLUDE Q16eChk [1]

*QUESTION Q16e *MR *INCLUDE [Q13abc] *IF [NumberOfResponse[Q16eChk]=2 | NumberOfResponse[Q14a]>NumberOfResponse[Q16bDummy]]
Q16e. It seems that you have used some of the credit cards in the past which you may not be using now. Please think again and tell me which credit card have you used before your existing cards [MA] 
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
99:No I have not used any Credit Cards before *NMUL *NOCON

*STARTREC "Q17a"

*QUESTION Q17a *SR *INCLUDE [Q16bDummy] *IF [NumberOfResponse[Q16bDummy]>1]
Q17a. Which of these banks is your preferred bank for Credit Card? [SA]
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}

*ENDREC

*IF [NumberOfResponse[Q16bDummy]=1 & (QHD12d=1 | QHD12d=2)] *INCLUDE Q17a Q16bDummy

*GRIDLIST "GridCon"
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*QUESTION Q17b1 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=1] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.1 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.1}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
1:American Express Bank *PICT "01AEB.jpg"


*QUESTION Q17b2 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=2] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.2}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
2:Axis Bank *PICT "02AB.jpg"

*QUESTION Q17b3 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=3] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.3}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
3:Bandhan Bank *PICT "03BdhB.jpg"

*QUESTION Q17b4 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=4] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.4}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
4:Bank of Baroda *PICT "04BB.jpg"

*QUESTION Q17b5 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=5] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.5}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
5:Bank of India *PICT "05BI.jpg"

*QUESTION Q17b6 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=6] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.6}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
6:Bank of Maharashtra *PICT "06BM.jpg"

*QUESTION Q17b7 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=7] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.7}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
7:Canara Bank *PICT "07CB.jpg"

*QUESTION Q17b8 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=8] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.8}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
8:Citibank *PICT "08CB.jpg"

*QUESTION Q17b9 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=9] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.9}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
9:DBS *PICT "09DBS.jpg"

*QUESTION Q17b10 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=10] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.10}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
10:HDFC Bank *PICT "10HDFCB.jpg"

*QUESTION Q17b11 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=11] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.11}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
11:HSBC *PICT "11HSBCB.jpg"

*QUESTION Q17b12 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=12] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.12}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
12:ICICI Bank *PICT "12ICICIB.jpg"

*QUESTION Q17b13 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=13] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.13}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
13:IDFC First Bank *PICT "13IDFCFB.jpg"

*QUESTION Q17b14 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=14] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.14}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
14:IndusInd Bank *PICT "14IB.jpg"

*QUESTION Q17b15 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=15] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.15}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
15:Kotak Mahindra Bank *PICT "15KB.jpg"

*QUESTION Q17b16 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=16] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.16}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
16:Punjab National Bank *PICT "16PNB.jpg"

*QUESTION Q17b17 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=17] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.17}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
17:Standard Chartered Bank *PICT "17SCB.jpg"

*QUESTION Q17b18 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=18] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.18}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
18:State Bank of India / SBI *PICT "18SBI.jpg"

*QUESTION Q17b19 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=19] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.19}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
19:YES Bank *PICT "19YB.jpg"

*QUESTION Q17b95 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=95] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.95}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
95:{Q13a.95}

*QUESTION Q17b96 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=96] *SHOWASFORM
<b>[SHOW CARD ]</b><br>
Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.96}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
96:{Q13b.96}

#*QUESTION Q17b97 *GRIDSR *USEGRIDLIST "GridCon" *IF [Q13abc=97] *SHOWASFORM
#<b>[SHOW CARD ]</b><br>
#Q17b.2 Now think of the next time you take any credit card, how likely are you to consider choosing <font color="#FF73FF">{Q13abc.97}</font> for credit card?  Please tell which statement best explains your action. SA FOR EACH BRAND. 
#<br>INTERVIEWER TO READ OUT EACH BRAND ONE BY ONE
#97:{Q13b.97}

*QUESTION DummyQ17c *MR *DUMMY2
Brand for Q17c
1:American Express Bank
2:Axis Bank
3:Bandhan Bank
4:Bank of Baroda
5:Bank of India
6:Bank of Maharashtra
7:Canara Bank
8:Citibank
9:DBS 
10:HDFC Bank
11:HSBC
12:ICICI Bank
13:IDFC First Bank
14:IndusInd Bank
15:Kotak Mahindra Bank
16:Punjab National Bank
17:Standard Chartered Bank
18:State Bank of India / SBI
19:YES Bank
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}


*IF [ValueOf[Q17b1.1]=1]   *INCLUDE DummyQ17c [1]
*IF [ValueOf[Q17b2.2]=1]   *INCLUDE DummyQ17c [2]
*IF [ValueOf[Q17b3.3]=1]   *INCLUDE DummyQ17c [3]
*IF [ValueOf[Q17b4.4]=1]   *INCLUDE DummyQ17c [4]
*IF [ValueOf[Q17b5.5]=1]   *INCLUDE DummyQ17c [5]
*IF [ValueOf[Q17b6.6]=1]   *INCLUDE DummyQ17c [6]
*IF [ValueOf[Q17b7.7]=1]   *INCLUDE DummyQ17c [7]
*IF [ValueOf[Q17b8.8]=1]   *INCLUDE DummyQ17c [8]
*IF [ValueOf[Q17b9.9]=1]   *INCLUDE DummyQ17c [9]
*IF [ValueOf[Q17b10.10]=1] *INCLUDE DummyQ17c [10]
*IF [ValueOf[Q17b11.11]=1] *INCLUDE DummyQ17c [11]
*IF [ValueOf[Q17b12.12]=1] *INCLUDE DummyQ17c [12]
*IF [ValueOf[Q17b13.13]=1] *INCLUDE DummyQ17c [13]
*IF [ValueOf[Q17b14.14]=1] *INCLUDE DummyQ17c [14]
*IF [ValueOf[Q17b15.15]=1] *INCLUDE DummyQ17c [15]
*IF [ValueOf[Q17b16.16]=1] *INCLUDE DummyQ17c [16]
*IF [ValueOf[Q17b17.17]=1] *INCLUDE DummyQ17c [17]
*IF [ValueOf[Q17b18.18]=1] *INCLUDE DummyQ17c [18]
*IF [ValueOf[Q17b19.19]=1] *INCLUDE DummyQ17c [19]
*IF [ValueOf[Q17b95.95]=1] *INCLUDE DummyQ17c [95]
*IF [ValueOf[Q17b96.96]=1] *INCLUDE DummyQ17c [96]
*#IF [ValueOf[Q17b97.97]=1] *INCLUDE DummyQ17c [97]

#*QUESTION Q17bChk *INFO *IF [NumberOfResponse[DummyQ17c]=0] *NONEXTBTN
#No Bnak is coded for <b>This is the only bank for credit card that I prefer</b>
#<br>Please go back and check the answer again.


*QUESTION Q17c *SR *INCLUDE [DummyQ17c] *IF [NumberOfResponse[DummyQ17c]>1]
[SHOW SCREEN]<br>
Q17c. Of these banks of credit cards, if you have to choose one, which bank of credit card would you prefer the most? [SA]
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*IF [NumberOfResponse[DummyQ17c]=1] *INCLUDE Q17c DummyQ17c


*QUESTION Q17e1 *SR *DUMMY2
B1. Consideration - American Express Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=1] *INCLUDE Q17e1 [1]
*IF [Q17c!=1 & ValueOf[Q17b1.1]=1] *INCLUDE Q17e1 [2]
*IF [Q17c!=1 & ValueOf[Q17b1.1]=2] *INCLUDE Q17e1 [2]
*IF [Q17c!=1 & ValueOf[Q17b1.1]=3] *INCLUDE Q17e1 [3]
*IF [Q17c!=1 & ValueOf[Q17b1.1]=4] *INCLUDE Q17e1 [4]
*IF [Q17c!=1 & ValueOf[Q17b1.1]=5] *INCLUDE Q17e1 [5]
*IF [Q17c!=1 & ValueOf[Q17b1.1]=9] *INCLUDE Q17e1 [9]

*QUESTION Q17e2 *SR *DUMMY2
B2. Consideration - Axis Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=2] *INCLUDE Q17e2 [1]
*IF [Q17c!=2 & ValueOf[Q17b2.2]=1] *INCLUDE Q17e2 [2]
*IF [Q17c!=2 & ValueOf[Q17b2.2]=2] *INCLUDE Q17e2 [2]
*IF [Q17c!=2 & ValueOf[Q17b2.2]=3] *INCLUDE Q17e2 [3]
*IF [Q17c!=2 & ValueOf[Q17b2.2]=4] *INCLUDE Q17e2 [4]
*IF [Q17c!=2 & ValueOf[Q17b2.2]=5] *INCLUDE Q17e2 [5]
*IF [Q17c!=2 & ValueOf[Q17b2.2]=9] *INCLUDE Q17e2 [9]

*QUESTION Q17e3 *SR *DUMMY2
B3. Consideration - Bandhan Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=3] *INCLUDE Q17e3 [1]
*IF [Q17c!=3 & ValueOf[Q17b3.3]=1] *INCLUDE Q17e3 [2]
*IF [Q17c!=3 & ValueOf[Q17b3.3]=2] *INCLUDE Q17e3 [2]
*IF [Q17c!=3 & ValueOf[Q17b3.3]=3] *INCLUDE Q17e3 [3]
*IF [Q17c!=3 & ValueOf[Q17b3.3]=4] *INCLUDE Q17e3 [4]
*IF [Q17c!=3 & ValueOf[Q17b3.3]=5] *INCLUDE Q17e3 [5]
*IF [Q17c!=3 & ValueOf[Q17b3.3]=9] *INCLUDE Q17e3 [9]

*QUESTION Q17e4 *SR *DUMMY2
B4. Consideration - Bank of Baroda
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=4] *INCLUDE Q17e4 [1]
*IF [Q17c!=4 & ValueOf[Q17b4.4]=1] *INCLUDE Q17e4 [2]
*IF [Q17c!=4 & ValueOf[Q17b4.4]=2] *INCLUDE Q17e4 [2]
*IF [Q17c!=4 & ValueOf[Q17b4.4]=3] *INCLUDE Q17e4 [3]
*IF [Q17c!=4 & ValueOf[Q17b4.4]=4] *INCLUDE Q17e4 [4]
*IF [Q17c!=4 & ValueOf[Q17b4.4]=5] *INCLUDE Q17e4 [5]
*IF [Q17c!=4 & ValueOf[Q17b4.4]=9] *INCLUDE Q17e4 [9]

*QUESTION Q17e5 *SR *DUMMY2
B5. Consideration - Bank of India
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=5] *INCLUDE Q17e5 [1]
*IF [Q17c!=5 & ValueOf[Q17b5.5]=1] *INCLUDE Q17e5 [2]
*IF [Q17c!=5 & ValueOf[Q17b5.5]=2] *INCLUDE Q17e5 [2]
*IF [Q17c!=5 & ValueOf[Q17b5.5]=3] *INCLUDE Q17e5 [3]
*IF [Q17c!=5 & ValueOf[Q17b5.5]=4] *INCLUDE Q17e5 [4]
*IF [Q17c!=5 & ValueOf[Q17b5.5]=5] *INCLUDE Q17e5 [5]
*IF [Q17c!=5 & ValueOf[Q17b5.5]=9] *INCLUDE Q17e5 [9]

*QUESTION Q17e6 *SR *DUMMY2
B6. Consideration - Bank of Maharashtra
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=6] *INCLUDE Q17e6 [1]
*IF [Q17c!=6 & ValueOf[Q17b6.6]=1] *INCLUDE Q17e6 [2]
*IF [Q17c!=6 & ValueOf[Q17b6.6]=2] *INCLUDE Q17e6 [2]
*IF [Q17c!=6 & ValueOf[Q17b6.6]=3] *INCLUDE Q17e6 [3]
*IF [Q17c!=6 & ValueOf[Q17b6.6]=4] *INCLUDE Q17e6 [4]
*IF [Q17c!=6 & ValueOf[Q17b6.6]=5] *INCLUDE Q17e6 [5]
*IF [Q17c!=6 & ValueOf[Q17b6.6]=9] *INCLUDE Q17e6 [9]

*QUESTION Q17e7 *SR *DUMMY2
B7. Consideration - Canara Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=7] *INCLUDE Q17e7 [1]
*IF [Q17c!=7 & ValueOf[Q17b7.7]=1] *INCLUDE Q17e7 [2]
*IF [Q17c!=7 & ValueOf[Q17b7.7]=2] *INCLUDE Q17e7 [2]
*IF [Q17c!=7 & ValueOf[Q17b7.7]=3] *INCLUDE Q17e7 [3]
*IF [Q17c!=7 & ValueOf[Q17b7.7]=4] *INCLUDE Q17e7 [4]
*IF [Q17c!=7 & ValueOf[Q17b7.7]=5] *INCLUDE Q17e7 [5]
*IF [Q17c!=7 & ValueOf[Q17b7.7]=9] *INCLUDE Q17e7 [9]

*QUESTION Q17e8 *SR *DUMMY2
B8. Consideration - Citibank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=8] *INCLUDE Q17e8 [1]
*IF [Q17c!=8 & ValueOf[Q17b8.8]=1] *INCLUDE Q17e8 [2]
*IF [Q17c!=8 & ValueOf[Q17b8.8]=2] *INCLUDE Q17e8 [2]
*IF [Q17c!=8 & ValueOf[Q17b8.8]=3] *INCLUDE Q17e8 [3]
*IF [Q17c!=8 & ValueOf[Q17b8.8]=4] *INCLUDE Q17e8 [4]
*IF [Q17c!=8 & ValueOf[Q17b8.8]=5] *INCLUDE Q17e8 [5]
*IF [Q17c!=8 & ValueOf[Q17b8.8]=9] *INCLUDE Q17e8 [9]

*QUESTION Q17e9 *SR *DUMMY2
B9. Consideration - DBS 
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=9] *INCLUDE Q17e9 [1]
*IF [Q17c!=9 & ValueOf[Q17b9.9]=1] *INCLUDE Q17e9 [2]
*IF [Q17c!=9 & ValueOf[Q17b9.9]=2] *INCLUDE Q17e9 [2]
*IF [Q17c!=9 & ValueOf[Q17b9.9]=3] *INCLUDE Q17e9 [3]
*IF [Q17c!=9 & ValueOf[Q17b9.9]=4] *INCLUDE Q17e9 [4]
*IF [Q17c!=9 & ValueOf[Q17b9.9]=5] *INCLUDE Q17e9 [5]
*IF [Q17c!=9 & ValueOf[Q17b9.9]=9] *INCLUDE Q17e9 [9]

*QUESTION Q17e10 *SR *DUMMY2
B10. Consideration - HDFC Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=10] *INCLUDE Q17e10 [1]
*IF [Q17c!=10 & ValueOf[Q17b10.10]=1] *INCLUDE Q17e10 [2]
*IF [Q17c!=10 & ValueOf[Q17b10.10]=2] *INCLUDE Q17e10 [2]
*IF [Q17c!=10 & ValueOf[Q17b10.10]=3] *INCLUDE Q17e10 [3]
*IF [Q17c!=10 & ValueOf[Q17b10.10]=4] *INCLUDE Q17e10 [4]
*IF [Q17c!=10 & ValueOf[Q17b10.10]=5] *INCLUDE Q17e10 [5]
*IF [Q17c!=10 & ValueOf[Q17b10.10]=9] *INCLUDE Q17e10 [9]

*QUESTION Q17e11 *SR *DUMMY2
B11. Consideration - HSBC
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=11] *INCLUDE Q17e11 [1]
*IF [Q17c!=11 & ValueOf[Q17b11.11]=1] *INCLUDE Q17e11 [2]
*IF [Q17c!=11 & ValueOf[Q17b11.11]=2] *INCLUDE Q17e11 [2]
*IF [Q17c!=11 & ValueOf[Q17b11.11]=3] *INCLUDE Q17e11 [3]
*IF [Q17c!=11 & ValueOf[Q17b11.11]=4] *INCLUDE Q17e11 [4]
*IF [Q17c!=11 & ValueOf[Q17b11.11]=5] *INCLUDE Q17e11 [5]
*IF [Q17c!=11 & ValueOf[Q17b11.11]=9] *INCLUDE Q17e11 [9]

*QUESTION Q17e12 *SR *DUMMY2
B12. Consideration - ICICI Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=12] *INCLUDE Q17e12 [1]
*IF [Q17c!=12 & ValueOf[Q17b12.12]=1] *INCLUDE Q17e12 [2]
*IF [Q17c!=12 & ValueOf[Q17b12.12]=2] *INCLUDE Q17e12 [2]
*IF [Q17c!=12 & ValueOf[Q17b12.12]=3] *INCLUDE Q17e12 [3]
*IF [Q17c!=12 & ValueOf[Q17b12.12]=4] *INCLUDE Q17e12 [4]
*IF [Q17c!=12 & ValueOf[Q17b12.12]=5] *INCLUDE Q17e12 [5]
*IF [Q17c!=12 & ValueOf[Q17b12.12]=9] *INCLUDE Q17e12 [9]

*QUESTION Q17e13 *SR *DUMMY2
B13. Consideration - IDFC First Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=13] *INCLUDE Q17e13 [1]
*IF [Q17c!=13 & ValueOf[Q17b13.13]=1] *INCLUDE Q17e13 [2]
*IF [Q17c!=13 & ValueOf[Q17b13.13]=2] *INCLUDE Q17e13 [2]
*IF [Q17c!=13 & ValueOf[Q17b13.13]=3] *INCLUDE Q17e13 [3]
*IF [Q17c!=13 & ValueOf[Q17b13.13]=4] *INCLUDE Q17e13 [4]
*IF [Q17c!=13 & ValueOf[Q17b13.13]=5] *INCLUDE Q17e13 [5]
*IF [Q17c!=13 & ValueOf[Q17b13.13]=9] *INCLUDE Q17e13 [9]

*QUESTION Q17e14 *SR *DUMMY2
B14. Consideration - IndusInd Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=14] *INCLUDE Q17e14 [1]
*IF [Q17c!=14 & ValueOf[Q17b14.14]=1] *INCLUDE Q17e14 [2]
*IF [Q17c!=14 & ValueOf[Q17b14.14]=2] *INCLUDE Q17e14 [2]
*IF [Q17c!=14 & ValueOf[Q17b14.14]=3] *INCLUDE Q17e14 [3]
*IF [Q17c!=14 & ValueOf[Q17b14.14]=4] *INCLUDE Q17e14 [4]
*IF [Q17c!=14 & ValueOf[Q17b14.14]=5] *INCLUDE Q17e14 [5]
*IF [Q17c!=14 & ValueOf[Q17b14.14]=9] *INCLUDE Q17e14 [9]

*QUESTION Q17e15 *SR *DUMMY2
B15. Consideration - Kotak Mahindra Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=15] *INCLUDE Q17e15 [1]
*IF [Q17c!=15 & ValueOf[Q17b15.15]=1] *INCLUDE Q17e15 [2]
*IF [Q17c!=15 & ValueOf[Q17b15.15]=2] *INCLUDE Q17e15 [2]
*IF [Q17c!=15 & ValueOf[Q17b15.15]=3] *INCLUDE Q17e15 [3]
*IF [Q17c!=15 & ValueOf[Q17b15.15]=4] *INCLUDE Q17e15 [4]
*IF [Q17c!=15 & ValueOf[Q17b15.15]=5] *INCLUDE Q17e15 [5]
*IF [Q17c!=15 & ValueOf[Q17b15.15]=9] *INCLUDE Q17e15 [9]

*QUESTION Q17e16 *SR *DUMMY2
B16. Consideration - Punjab National Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=16] *INCLUDE Q17e16 [1]
*IF [Q17c!=16 & ValueOf[Q17b16.16]=1] *INCLUDE Q17e16 [2]
*IF [Q17c!=16 & ValueOf[Q17b16.16]=2] *INCLUDE Q17e16 [2]
*IF [Q17c!=16 & ValueOf[Q17b16.16]=3] *INCLUDE Q17e16 [3]
*IF [Q17c!=16 & ValueOf[Q17b16.16]=4] *INCLUDE Q17e16 [4]
*IF [Q17c!=16 & ValueOf[Q17b16.16]=5] *INCLUDE Q17e16 [5]
*IF [Q17c!=16 & ValueOf[Q17b16.16]=9] *INCLUDE Q17e16 [9]

*QUESTION Q17e17 *SR *DUMMY2
B17. Consideration - Standard Chartered Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=17] *INCLUDE Q17e17 [1]
*IF [Q17c!=17 & ValueOf[Q17b17.17]=1] *INCLUDE Q17e17 [2]
*IF [Q17c!=17 & ValueOf[Q17b17.17]=2] *INCLUDE Q17e17 [2]
*IF [Q17c!=17 & ValueOf[Q17b17.17]=3] *INCLUDE Q17e17 [3]
*IF [Q17c!=17 & ValueOf[Q17b17.17]=4] *INCLUDE Q17e17 [4]
*IF [Q17c!=17 & ValueOf[Q17b17.17]=5] *INCLUDE Q17e17 [5]
*IF [Q17c!=17 & ValueOf[Q17b17.17]=9] *INCLUDE Q17e17 [9]

*QUESTION Q17e18 *SR *DUMMY2
B18. Consideration - State Bank of India / SBI
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=18] *INCLUDE Q17e18 [1]
*IF [Q17c!=18 & ValueOf[Q17b18.18]=1] *INCLUDE Q17e18 [2]
*IF [Q17c!=18 & ValueOf[Q17b18.18]=2] *INCLUDE Q17e18 [2]
*IF [Q17c!=18 & ValueOf[Q17b18.18]=3] *INCLUDE Q17e18 [3]
*IF [Q17c!=18 & ValueOf[Q17b18.18]=4] *INCLUDE Q17e18 [4]
*IF [Q17c!=18 & ValueOf[Q17b18.18]=5] *INCLUDE Q17e18 [5]
*IF [Q17c!=18 & ValueOf[Q17b18.18]=9] *INCLUDE Q17e18 [9]

*QUESTION Q17e19 *SR *DUMMY2
B19. Consideration - YES Bank
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=19] *INCLUDE Q17e19 [1]
*IF [Q17c!=19 & ValueOf[Q17b19.19]=1] *INCLUDE Q17e19 [2]
*IF [Q17c!=19 & ValueOf[Q17b19.19]=2] *INCLUDE Q17e19 [2]
*IF [Q17c!=19 & ValueOf[Q17b19.19]=3] *INCLUDE Q17e19 [3]
*IF [Q17c!=19 & ValueOf[Q17b19.19]=4] *INCLUDE Q17e19 [4]
*IF [Q17c!=19 & ValueOf[Q17b19.19]=5] *INCLUDE Q17e19 [5]
*IF [Q17c!=19 & ValueOf[Q17b19.19]=9] *INCLUDE Q17e19 [9]

*QUESTION Q17e95 *SR *DUMMY2
B95. Consideration - {Q13a.95}
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=95] *INCLUDE Q17e95 [1]
*IF [Q17c!=95 & ValueOf[Q17b95.95]=1] *INCLUDE Q17e95 [2]
*IF [Q17c!=95 & ValueOf[Q17b95.95]=2] *INCLUDE Q17e95 [2]
*IF [Q17c!=95 & ValueOf[Q17b95.95]=3] *INCLUDE Q17e95 [3]
*IF [Q17c!=95 & ValueOf[Q17b95.95]=4] *INCLUDE Q17e95 [4]
*IF [Q17c!=95 & ValueOf[Q17b95.95]=5] *INCLUDE Q17e95 [5]
*IF [Q17c!=95 & ValueOf[Q17b95.95]=9] *INCLUDE Q17e95 [9]

*QUESTION Q17e96 *SR *DUMMY2
B96. Consideration - {Q13b.96}
1:This is the only bank for credit card that I prefer
2:This is one of 2 or 3 banks for credit card that I prefer 
3:This is one of the several banks for credit card  that I will consider
4:This is a bank for credit card I might consider 
5:This is a bank for credit card I will not consider 
9:Don't know/ Can't Say

*IF [Q17c=96] *INCLUDE Q17e96 [1]
*IF [Q17c!=96 & ValueOf[Q17b96.96]=1] *INCLUDE Q17e96 [2]
*IF [Q17c!=96 & ValueOf[Q17b96.96]=2] *INCLUDE Q17e96 [2]
*IF [Q17c!=96 & ValueOf[Q17b96.96]=3] *INCLUDE Q17e96 [3]
*IF [Q17c!=96 & ValueOf[Q17b96.96]=4] *INCLUDE Q17e96 [4]
*IF [Q17c!=96 & ValueOf[Q17b96.96]=5] *INCLUDE Q17e96 [5]
*IF [Q17c!=96 & ValueOf[Q17b96.96]=9] *INCLUDE Q17e96 [9]

#*QUESTION Q17e97 *SR *DUMMY2
#B97. Consideration - {Q13b.97}
#1:This is the only bank for credit card that I prefer
#2:This is one of 2 or 3 banks for credit card that I prefer 
#3:This is one of the several banks for credit card  that I will consider
#4:This is a bank for credit card I might consider 
#5:This is a bank for credit card I will not consider 
#9:Don't know/ Can't Say
#
#*IF [Q17c=97] *INCLUDE Q17e97 [1]
#*IF [Q17c!=97 & ValueOf[Q17b97.97]=1] *INCLUDE Q17e97 [2]
#*IF [Q17c!=97 & ValueOf[Q17b97.97]=2] *INCLUDE Q17e97 [2]
#*IF [Q17c!=97 & ValueOf[Q17b97.97]=3] *INCLUDE Q17e97 [3]
#*IF [Q17c!=97 & ValueOf[Q17b97.97]=4] *INCLUDE Q17e97 [4]
#*IF [Q17c!=97 & ValueOf[Q17b97.97]=5] *INCLUDE Q17e97 [5]
#*IF [Q17c!=97 & ValueOf[Q17b97.97]=9] *INCLUDE Q17e97 [9]


*QUESTION DummyQ18 *MR *DUMMY2
Brand for Q18
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*IF [Q13abc=1 & Q17b1!=5] *INCLUDE DummyQ18 [1]
*IF [Q13abc=2 & Q17b2!=5] *INCLUDE DummyQ18 [2]
*IF [Q13abc=3 & Q17b3!=5] *INCLUDE DummyQ18 [3]
*IF [Q13abc=4 & Q17b4!=5] *INCLUDE DummyQ18 [4]
*IF [Q13abc=5 & Q17b5!=5] *INCLUDE DummyQ18 [5]
*IF [Q13abc=6 & Q17b6!=5] *INCLUDE DummyQ18 [6]
*IF [Q13abc=7 & Q17b7!=5] *INCLUDE DummyQ18 [7]
*IF [Q13abc=8 & Q17b8!=5] *INCLUDE DummyQ18 [8]
*IF [Q13abc=9 & Q17b9!=5] *INCLUDE DummyQ18 [9]
*IF [Q13abc=10 & Q17b10!=5] *INCLUDE DummyQ18 [10]
*IF [Q13abc=11 & Q17b11!=5] *INCLUDE DummyQ18 [11]
*IF [Q13abc=12 & Q17b12!=5] *INCLUDE DummyQ18 [12]
*IF [Q13abc=13 & Q17b13!=5] *INCLUDE DummyQ18 [13]
*IF [Q13abc=14 & Q17b14!=5] *INCLUDE DummyQ18 [14]
*IF [Q13abc=15 & Q17b15!=5] *INCLUDE DummyQ18 [15]
*IF [Q13abc=16 & Q17b16!=5] *INCLUDE DummyQ18 [16]
*IF [Q13abc=17 & Q17b17!=5] *INCLUDE DummyQ18 [17]
*IF [Q13abc=18 & Q17b18!=5] *INCLUDE DummyQ18 [18]
*IF [Q13abc=19 & Q17b19!=5] *INCLUDE DummyQ18 [19]
*IF [Q13abc=95 & Q17b95!=5] *INCLUDE DummyQ18 [95]
*IF [Q13abc=96 & Q17b96!=5] *INCLUDE DummyQ18 [96]
#*IF [Q13abc=97 & Q17b97!=5] *INCLUDE DummyQ18 [97]


*QUESTION Q18 *SR *INCLUDE [DummyQ18] *COLUMN 2 *RANDOM *IF [NumberOfResponse[Q13abc]>0 & (QHD12d=2 | QHD12d=3)]
Q18. As you are looking to take a credit card in the next 6 months, from which of these banks are you most likely to buy a credit card in the next 6 months? [SA]
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

################################################Imagery####################################################
################################################Imagery####################################################


*QUESTION DummyImgBr1 *MR *DUMMY2
DummyImgBr1
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*IF [Q13abc=10] *INCLUDE DummyImgBr1 [10] 
*IF [Q13abc=12] *INCLUDE DummyImgBr1 [12] 
*IF [Q13abc=2] *INCLUDE DummyImgBr1 [2] 
*IF [Q13abc=15] *INCLUDE DummyImgBr1 [15] 
*IF [Q13abc=18] *INCLUDE DummyImgBr1 [18]
*IF [Q13abc=8] *INCLUDE DummyImgBr1 [8] 
*IF [Q13abc=1] *INCLUDE DummyImgBr1 [1]




*QUESTION DummyImgBr2 *MR *DUMMY2
DummyImgBr2
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*INCLUDE DummyImgBr2 DummyImgBr1
*IF [NumberOfResponse[DummyImgBr1]<7] *INCLUDE DummyImgBr2 Q17a



*QUESTION DummyImgBr3 *MR *DUMMY2
DummyImgBr3
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*INCLUDE DummyImgBr3 DummyImgBr2
*IF [NumberOfResponse[DummyImgBr2]<7] *INCLUDE DummyImgBr3 Q18


*QUESTION DummyQ16aEx *MR *DUMMY2
DummyQ16a Exclude Q17a Q18
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

#*IF [Q16bDummy=1 & Q17a!=1 & Q18!=1] *INCLUDE DummyQ16aEx [1]
#*IF [Q16bDummy=2 & Q17a!=2 & Q18!=2] *INCLUDE DummyQ16aEx [2]
*IF [Q16bDummy=3 & Q17a!=3 & Q18!=3] *INCLUDE DummyQ16aEx [3]
*IF [Q16bDummy=4 & Q17a!=4 & Q18!=4] *INCLUDE DummyQ16aEx [4]
*IF [Q16bDummy=5 & Q17a!=5 & Q18!=5] *INCLUDE DummyQ16aEx [5]
*IF [Q16bDummy=6 & Q17a!=6 & Q18!=6] *INCLUDE DummyQ16aEx [6]
*IF [Q16bDummy=7 & Q17a!=7 & Q18!=7] *INCLUDE DummyQ16aEx [7]
#*IF [Q16bDummy=8 & Q17a!=8 & Q18!=8] *INCLUDE DummyQ16aEx [8]
*IF [Q16bDummy=9 & Q17a!=9 & Q18!=9] *INCLUDE DummyQ16aEx [9]
#*IF [Q16bDummy=10 & Q17a!=10 & Q18!=10] *INCLUDE DummyQ16aEx [10]
*IF [Q16bDummy=11 & Q17a!=11 & Q18!=11] *INCLUDE DummyQ16aEx [11]
#*IF [Q16bDummy=12 & Q17a!=12 & Q18!=12] *INCLUDE DummyQ16aEx [12]
*IF [Q16bDummy=13 & Q17a!=13 & Q18!=13] *INCLUDE DummyQ16aEx [13]
*IF [Q16bDummy=14 & Q17a!=14 & Q18!=14] *INCLUDE DummyQ16aEx [14]
#*IF [Q16bDummy=15 & Q17a!=15 & Q18!=15] *INCLUDE DummyQ16aEx [15]
*IF [Q16bDummy=16 & Q17a!=16 & Q18!=16] *INCLUDE DummyQ16aEx [16]
*IF [Q16bDummy=17 & Q17a!=17 & Q18!=17] *INCLUDE DummyQ16aEx [17]
#*IF [Q16bDummy=18 & Q17a!=18 & Q18!=18] *INCLUDE DummyQ16aEx [18]
*IF [Q16bDummy=19 & Q17a!=19 & Q18!=19] *INCLUDE DummyQ16aEx [19]
*IF [Q16bDummy=95 & Q17a!=95 & Q18!=95] *INCLUDE DummyQ16aEx [95]
*IF [Q16bDummy=96 & Q17a!=96 & Q18!=96] *INCLUDE DummyQ16aEx [96]
*IF [Q16bDummy=97 & Q17a!=97 & Q18!=97] *INCLUDE DummyQ16aEx [97]




*QUESTION DummyImgBr4 *MR *DUMMY2
DummyImgBr4
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}



*INCLUDE DummyImgBr4 DummyImgBr3
*IF [NumberOfResponse[DummyImgBr3]=1 & NumberOfResponse[DummyQ16aEx]<7] *INCLUDE DummyImgBr4 DummyQ16aEx
*IF [NumberOfResponse[DummyImgBr3]=2 & NumberOfResponse[DummyQ16aEx]<6] *INCLUDE DummyImgBr4 DummyQ16aEx
*IF [NumberOfResponse[DummyImgBr3]=3 & NumberOfResponse[DummyQ16aEx]<5] *INCLUDE DummyImgBr4 DummyQ16aEx
*IF [NumberOfResponse[DummyImgBr3]=4 & NumberOfResponse[DummyQ16aEx]<4] *INCLUDE DummyImgBr4 DummyQ16aEx
*IF [NumberOfResponse[DummyImgBr3]=5 & NumberOfResponse[DummyQ16aEx]<3] *INCLUDE DummyImgBr4 DummyQ16aEx
*IF [NumberOfResponse[DummyImgBr3]=6 & NumberOfResponse[DummyQ16aEx]<2] *INCLUDE DummyImgBr4 DummyQ16aEx

*IF [NumberOfResponse[DummyImgBr3]=1 & NumberOfResponse[DummyQ16aEx]>6] *INCLUDE DummyImgBr4 RandomValueOf[DummyQ16aEx,6]
*IF [NumberOfResponse[DummyImgBr3]=2 & NumberOfResponse[DummyQ16aEx]>5] *INCLUDE DummyImgBr4 RandomValueOf[DummyQ16aEx,5]
*IF [NumberOfResponse[DummyImgBr3]=3 & NumberOfResponse[DummyQ16aEx]>4] *INCLUDE DummyImgBr4 RandomValueOf[DummyQ16aEx,4]
*IF [NumberOfResponse[DummyImgBr3]=4 & NumberOfResponse[DummyQ16aEx]>3] *INCLUDE DummyImgBr4 RandomValueOf[DummyQ16aEx,3]
*IF [NumberOfResponse[DummyImgBr3]=5 & NumberOfResponse[DummyQ16aEx]>2] *INCLUDE DummyImgBr4 RandomValueOf[DummyQ16aEx,2]
*IF [NumberOfResponse[DummyImgBr3]=6 & NumberOfResponse[DummyQ16aEx]>1] *INCLUDE DummyImgBr4 RandomValueOf[DummyQ16aEx,1]





*QUESTION DummyImgBr5 *MR *DUMMY2
DummyImgBr5
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*INCLUDE DummyImgBr5 Q13abc
*EXCLUDE DummyImgBr5 DummyImgBr4


*QUESTION FinalImgBr *MR *DUMMY2
Final Brand Image
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}


*INCLUDE FinalImgBr DummyImgBr4
*IF [NumberOfResponse[DummyImgBr4]=1 & NumberOfResponse[DummyImgBr5]<7] *INCLUDE FinalImgBr DummyImgBr5
*IF [NumberOfResponse[DummyImgBr4]=2 & NumberOfResponse[DummyImgBr5]<6] *INCLUDE FinalImgBr DummyImgBr5
*IF [NumberOfResponse[DummyImgBr4]=3 & NumberOfResponse[DummyImgBr5]<5] *INCLUDE FinalImgBr DummyImgBr5
*IF [NumberOfResponse[DummyImgBr4]=4 & NumberOfResponse[DummyImgBr5]<4] *INCLUDE FinalImgBr DummyImgBr5
*IF [NumberOfResponse[DummyImgBr4]=5 & NumberOfResponse[DummyImgBr5]<3] *INCLUDE FinalImgBr DummyImgBr5
*IF [NumberOfResponse[DummyImgBr4]=6 & NumberOfResponse[DummyImgBr5]<2] *INCLUDE FinalImgBr DummyImgBr5

*IF [NumberOfResponse[DummyImgBr4]=1 & NumberOfResponse[DummyImgBr5]>6] *INCLUDE FinalImgBr RandomValueOf[DummyImgBr5,6]
*IF [NumberOfResponse[DummyImgBr4]=2 & NumberOfResponse[DummyImgBr5]>5] *INCLUDE FinalImgBr RandomValueOf[DummyImgBr5,5]
*IF [NumberOfResponse[DummyImgBr4]=3 & NumberOfResponse[DummyImgBr5]>4] *INCLUDE FinalImgBr RandomValueOf[DummyImgBr5,4]
*IF [NumberOfResponse[DummyImgBr4]=4 & NumberOfResponse[DummyImgBr5]>3] *INCLUDE FinalImgBr RandomValueOf[DummyImgBr5,3]
*IF [NumberOfResponse[DummyImgBr4]=5 & NumberOfResponse[DummyImgBr5]>2] *INCLUDE FinalImgBr RandomValueOf[DummyImgBr5,2]
*IF [NumberOfResponse[DummyImgBr4]=6 & NumberOfResponse[DummyImgBr5]>1] *INCLUDE FinalImgBr RandomValueOf[DummyImgBr5,1]

*STARTREC "Q19"

*QUESTION Q19Info *INFO
Now I am going to read out a few statements which people like you have mentioned for Credit Card banks. For each statement, please indicate the Credit Card bank or banks it applies to. You can select as many or as few banks as you wish for each statements. 
<br>If you think that a statement does not apply to any brands, please select "None of these". [MA POSSIBLE FOR A STATEMENT]

*QUESTION Q191 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>The brand gives access to exclusive and premium benefits</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q192 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>The brand enhances my status/ image in my circle</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q193 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>This is a trustworthy brand</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q194 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>It is for people like me</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q195 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Heard & read a lot of good things / reviews about this brand</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q196 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>It is recommended by family/ friends/ relatives/ peers</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q197 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>This is an innovative brand that keeps on coming with new cards with unique offers & benefits</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q198 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>The brand offers attractive loyalty program with great reward points and multiple options to redeem from</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q199 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Is a bank that has prompt customer service across Mobile / email / branch/ RM</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q1910 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Has Competitive fees /charges on Credit Cards</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q1911 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Has an easy to use mobile interface/app</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q1912 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Is a brand that is an expert in offering Credit Cards for all needs</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q1913 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Is a brand that is transparent in its dealings with Credit Cards Customers</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

*QUESTION Q1914 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
<big><b>Is modern & progressive in credit card offering</b></big>
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:NONE OF THESE *NMUL *NOCON

#*QUESTION Q1915 *MR *INCLUDE [FinalImgBr] *QROT *COLUMN 2 *IF [NumberOfResponse[FinalImgBr]>0] *IMGADJBY 200
#<big><b>Is a bank that has prompt customer service across Mobile / email / branch/ RM</b></big>
#*USELIST "Q13abcListImg"
#95:{Q13a.95}
#96:{Q13b.96}
#97:{Q13b.97}
#99:NONE OF THESE *NMUL *NOCON

##############################################################################################

*QUESTION DummyQ20a *MR *DUMMY2
Brand for Q20a
1:Wide array of Credit card <b>benefits & offers</b>
2:<b>Competitive interest</b> rate on late payments 
3:Getting <b>timely reminder</b> about payments & fees
4:Offers <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand has been very good
6:Heard <b>good reviews</b> about this credit card from others 
7:<b>Trustworthy</b> brand
8:<b>Using technology</b> to better engage with clients and deliver on their expectation
9:Have <b>useful digital solution/ Interface</b> for Credit card 
10:I have my <b>saving account</b> with this bank
#11:The brand gives <b>shopping related offers</b> on e-Commerce platforms and in stores (electronics, fashion, groceries, etc..)
#12:Has best tie up for <b>domestic and international travel</b> relevant to me
#13:Has best tie ups with <b>travel booking</b> relevant to me
#14:Has best tie up for <b>dining & restaurants</b> relevant to me 
#15:Had best tie up of <b>Airport lounge access</b> relevant me
16:The brand gives <b>grace period</b> on payment due date/<b>flexibility</b> on payment date.
17:Have seen a <b>lot of people using this card</b>
18:Has best and relevant offers (for me) on <b>physical /instore shopping.</b>
19:Has best and relevant offers (for me) on <b>online shopping.</b>
20:Has best and relevant offers (for me) on <b>international hotels and flights.</b>
21:Has best and relevant offers (for me) on <b>domestic hotels and flights.</b>
22:Has best offers on <b>dining</b> which are relevant to me
23:Had best offers on <b>Airport lounge access</b> which are relevant to me
24:Has best offers for <b>entertainment /movies/concerts</b> etc
25:Has best <b>Rewards program</b>
26:Offers the best benefits for <b>Fuel expenses</b>
27:<b>Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
28:Provides waiver on Annual Fees / Lifetime free card
29:Provides better Value of <b>fees paid vs benefits earned</b>
98:Any other (Specify) *OPEN

*INCLUDE DummyQ20a [1 TO 16]
*INCLUDE DummyQ20a [18 TO 29]
*INCLUDE DummyQ20a [98]
*IF [QHD12d=2] *INCLUDE DummyQ20a [17]

*ENDREC

#*INCLUDE [DummyQ20a] NumberOfResponse[DummyQ20a]>0 &

*STARTREC "Q20aQ20b"

*QUESTION Q20a *RANK *MIN 8 *MAX 8 *IF [QHD12d=1 | QHD12d=2]
[Show Cards]
<br>Q20a.You said that <font color="#FF73FF">{Q17a}</font> is your preferred bank for credit card. What are the top 8 reasons for preferring <font color="#FF73FF">{Q17a}</font> for Credit card.
Pl rank them in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:Wide array of Credit card <b>benefits & offers</b>
2:<b>Competitive interest</b> rate on late payments 
3:Getting <b>timely reminder</b> about payments & fees
4:Offers <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand has been very good
6:Heard <b>good reviews</b> about this credit card from others 
7:<b>Trustworthy</b> brand
8:<b>Using technology</b> to better engage with clients and deliver on their expectation
9:Have <b>useful digital solution/ Interface</b> for Credit card 
10:I have my <b>saving account</b> with this bank
#11:The brand gives <b>shopping related offers</b> on e-Commerce platforms and in stores (electronics, fashion, groceries, etc..)
#12:Has best tie up for <b>domestic and international travel</b> relevant to me
#13:Has best tie ups with <b>travel booking</b> relevant to me
#14:Has best tie up for <b>dining & restaurants</b> relevant to me 
#15:Had best tie up of <b>Airport lounge access</b> relevant me
16:The brand gives <b>grace period</b> on payment due date/<b>flexibility</b> on payment date.
17:Have seen a <b>lot of people using this card</b>
18:Has best and relevant offers (for me) on <b>physical /instore shopping.</b>
19:Has best and relevant offers (for me) on <b>online shopping.</b>
20:Has best and relevant offers (for me) on <b>international hotels and flights.</b>
21:Has best and relevant offers (for me) on <b>domestic hotels and flights.</b>
22:Has best offers on <b>dining</b> which are relevant to me
23:Had best offers on <b>Airport lounge access</b> which are relevant to me
24:Has best offers for <b>entertainment /movies/concerts</b> etc
25:Has best <b>Rewards program</b>
26:Offers the best benefits for <b>Fuel expenses</b>
27:<b>Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
28:Provides waiver on Annual Fees / Lifetime free card
29:Provides better Value of <b>fees paid vs benefits earned</b>
98:Any other (Specify) *OPEN


*QUESTION Q20b *RANK *MIN 8 *MAX 8 *INCLUDE [DummyQ20a] *IF [(QHD12d=2 | QHD12d=3) & NumberOfResponse[DummyQ20a]>0]
[Show Cards]
<br>Q20b. You said that <font color="#FF73FF">{Q18}</font> is the bank from which you are most like to take a credit card. What are the top 8 reasons for preferring <font color="#FF73FF">{Q18}</font> for Credit card.
Pl rank them in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:Wide array of Credit card <b>benefits & offers</b>
2:<b>Competitive interest</b> rate on late payments 
3:Getting <b>timely reminder</b> about payments & fees
4:Offers <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand has been very good
6:Heard <b>good reviews</b> about this credit card from others 
7:<b>Trustworthy</b> brand
8:<b>Using technology</b> to better engage with clients and deliver on their expectation
9:Have <b>useful digital solution/ Interface</b> for Credit card 
10:I have my <b>saving account</b> with this bank
#11:The brand gives <b>shopping related offers</b> on e-Commerce platforms and in stores (electronics, fashion, groceries, etc..)
#12:Has best tie up for <b>domestic and international travel</b> relevant to me
#13:Has best tie ups with <b>travel booking</b> relevant to me
#14:Has best tie up for <b>dining & restaurants</b> relevant to me 
#15:Had best tie up of <b>Airport lounge access</b> relevant me
16:The brand gives <b>grace period</b> on payment due date/<b>flexibility</b> on payment date.
17:Have seen a <b>lot of people using this card</b>
18:Has best and relevant offers (for me) on <b>physical /instore shopping.</b>
19:Has best and relevant offers (for me) on <b>online shopping.</b>
20:Has best and relevant offers (for me) on <b>international hotels and flights.</b>
21:Has best and relevant offers (for me) on <b>domestic hotels and flights.</b>
22:Has best offers on <b>dining</b> which are relevant to me
23:Had best offers on <b>Airport lounge access</b> which are relevant to me
24:Has best offers for <b>entertainment /movies/concerts</b> etc
25:Has best <b>Rewards program</b>
26:Offers the best benefits for <b>Fuel expenses</b>
27:<b>Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
28:Provides waiver on Annual Fees / Lifetime free card
29:Provides better Value of <b>fees paid vs benefits earned</b>
98:Any other (Specify) *OPEN

*ENDREC

*QUESTION Q22 *MR *RANDOM *IF [QHD12d=3]
Q22. Can you tell me the reason for not taking a credit card so far for yourself?[MA]
1:I was not eligible for credit card/ Eligibility criteria concern
2:Fear of being a defaulter / not being able to pay on time
3:No clarity on due dates & how to pay back
4:No understanding of fees structure & charges
5:Do not want to depend on credit card for my expenses
6:Fear of over-spending if I get a credit card
7:Prefer paying through cash
8:Prefer paying through Debit card
9:Prefer paying through UPI
97:Any other (Specify) *OPEN
98:Any other (Specify) *OPEN


*QUESTION DummyQ23T1 *MR *DUMMY2
Temporary Brand for Q23
*USELIST "Q13abcList"
95:Others1
96:Others2
#97:Others3


*IF [ValueOf[Q17b1.1]=5 & Q16bDummy!=1] *INCLUDE DummyQ23T1 [1]
*IF [ValueOf[Q17b2.2]=5 & Q16bDummy!=2] *INCLUDE DummyQ23T1 [2]
*IF [ValueOf[Q17b3.3]=5 & Q16bDummy!=3] *INCLUDE DummyQ23T1 [3]
*IF [ValueOf[Q17b4.4]=5 & Q16bDummy!=4] *INCLUDE DummyQ23T1 [4]
*IF [ValueOf[Q17b5.5]=5 & Q16bDummy!=5] *INCLUDE DummyQ23T1 [5]
*IF [ValueOf[Q17b6.6]=5 & Q16bDummy!=6] *INCLUDE DummyQ23T1 [6]
*IF [ValueOf[Q17b7.7]=5 & Q16bDummy!=7] *INCLUDE DummyQ23T1 [7]
*IF [ValueOf[Q17b8.8]=5 & Q16bDummy!=8] *INCLUDE DummyQ23T1 [8]
*IF [ValueOf[Q17b9.9]=5 & Q16bDummy!=9] *INCLUDE DummyQ23T1 [9]
*IF [ValueOf[Q17b10.10]=5 & Q16bDummy!=10] *INCLUDE DummyQ23T1 [10]
*IF [ValueOf[Q17b11.11]=5 & Q16bDummy!=11] *INCLUDE DummyQ23T1 [11]
*IF [ValueOf[Q17b12.12]=5 & Q16bDummy!=12] *INCLUDE DummyQ23T1 [12]
*IF [ValueOf[Q17b13.13]=5 & Q16bDummy!=13] *INCLUDE DummyQ23T1 [13]
*IF [ValueOf[Q17b14.14]=5 & Q16bDummy!=14] *INCLUDE DummyQ23T1 [14]
*IF [ValueOf[Q17b15.15]=5 & Q16bDummy!=15] *INCLUDE DummyQ23T1 [15]
*IF [ValueOf[Q17b16.16]=5 & Q16bDummy!=16] *INCLUDE DummyQ23T1 [16]
*IF [ValueOf[Q17b17.17]=5 & Q16bDummy!=17] *INCLUDE DummyQ23T1 [17]
*IF [ValueOf[Q17b18.18]=5 & Q16bDummy!=18] *INCLUDE DummyQ23T1 [18]
*IF [ValueOf[Q17b19.19]=5 & Q16bDummy!=19] *INCLUDE DummyQ23T1 [19]
*IF [ValueOf[Q17b95.95]=5 & Q16bDummy!=95] *INCLUDE DummyQ23T1 [95]
*IF [ValueOf[Q17b96.96]=5 & Q16bDummy!=96] *INCLUDE DummyQ23T1 [96]
#*IF [ValueOf[Q17bx97.97]=5 & Q16bDummy!=97] *INCLUDE DummyQ23T1 [97]


*QUESTION DummyQ23 *SR *DUMMY2
Brand for Q23
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}

*IF [DummyQ23T1=2] *INCLUDE DummyQ23 [2]
*IF [DummyQ23T1!=2 & NumberOfResponse[DummyQ23T1]>0] *INCLUDE DummyQ23 RandomValueOf[DummyQ23T1,1]

### Do it later
*QUESTION Q23 *RANK *MIN 8 *MAX 8 *IF [NumberOfResponse[DummyQ23T1]>0]
[Show Cards]
Q23. You mentioned that you will not consider <font color="#FF73FF">{DummyQ23}</font> for credit card at all. What are the top 8 reasons for not considering <font color="#FF73FF">{DummyQ23}</font> for Credit card.
Please rank your reasons in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:The brand doesn't offer wide array of Credit card <b>benefits & offers</b>
2:Doesn't have <>bcompetitive interest</b> rate on late payments 
3:Doesn't provide <b>timely reminder</b> about payments & fees
4:Doesn't offer <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand were not good
6:<b>Heard bad reviews</b> about this credit card from others 
7:Is not a <b>trustworthy</b> brand
8:Haven't seen much of <b>technology usage</b> to better engage with clients and deliver on their expectation
9:Doesn't have useful <b>digital solution/ Interface</b> for Credit card 
10:Don't have any <b>prior association</b> with this bank hence won't consider
#11:The brand doesn't give many shopping related offers on eComm platforms and in stores (electronics, fashion, groceries, etc..)
#12:Doesn't have good tie up for domestic and international travel relevant to me
#13:Doesn't have good tie ups with travel booking relevant to me
#14:Doesn't have good tie up for dining & restaurants relevant to me 
#15:Doesn't have good tie up of Airport lounge access relevant me
16:<b>Haven't heard much</b> about the credit card offering of this brand
17:The brand doesn't give any <b>grace period</b> on payment due date/flexibility on payment date.
18:Does not have best and relevant offers (for me) on <b>online shopping.</b>
19:Does not have best and relevant offers (for me) on <b>international hotels and flights.</b>
20:Does not have best and relevant offers (for me) on <b>domestic hotels and flights.</b>
21:Does not have best offers on <b>dining</b> which are relevant to me
22:Does not have best offers on <b>Airport lounge access</b> which are relevant to me
23:Does not have best offers for <b>entertainment /movies/concerts</b> etc
24:Does not have best <b>Rewards program</b>
25:Does not Offer the best benefits for <b>Fuel expenses</b>
26:<b>No Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
27:Does not have Provide waiver on <b>Annual Fees / Lifetime free card</b>
28:Does not have Provide better Value of <b>fees paid vs benefits earned</b>
98:Any other (Specify) *OPEN


*QUESTION Q24 *MR *RANDOM *IF [QHD12d=1 | QHD12d=2]
<font color="#FF73FF">INSTRUCTION TO INTERVIEWER</font>
<font color="#FF73FF">IF THE RESPONDENT SAYS THEY USE <b>CREDIT CARD BANK APP</b> FOR REPAYMENT OR ANALYSIS, THEN SELECT "I DON'T USE ANY OF THESE APPS TO MANAGE MY CREDIT CARDS".</font>   
[SHOW SCREEN]
<br>Q24. Do you use any of the following third party apps to manage your Credit Cards for repayment or analysis purposes?
1:I don't use any of these apps to manage my Credit Cards *NMUL
2:CRED
3:Slice
4:CheQ
5:Paytm
6:Google Pay
7:Mobikwik
8:PhonePe
9:Amazon Pay
10:BharatNXT
11:Lazy Pay
98:Any other (Specify) *OPEN



###############################################################################

################################ Q25 New #################################

*QUESTION DummyQ25Axis *SR *DUMMY2
Axis Brand for Priority
*USELIST "Q13abcList"

*IF [Q13abc=2 & (Q17b2=1 | Q17b2=2) & Q16bDummy!=2 & Q17a!=2] *INCLUDE DummyQ25Axis [2]

*QUESTION DummyQ25HDFCIFIC *SR *DUMMY2
Axis Brand for Priority
*USELIST "Q13abcList"

*IF [Q13abc=10 & (Q17b10=1 | Q17b10=2) & Q16bDummy!=10 & Q17a!=10] *INCLUDE DummyQ25Axis [10]
*IF [Q13abc=12 & (Q17b12=1 | Q17b12=2) & Q16bDummy!=12 & Q17a!=12] *INCLUDE DummyQ25Axis [12]



*QUESTION DummyQ25 *MR *DUMMY2
Brand for Q25
*USELIST "Q13abcList"

*IF [DummyQ25Axis=2] *INCLUDE DummyQ25 DummyQ25Axis
*IF [DummyQ25Axis=2 & NumberOfResponse[DummyQ25HDFCIFIC]=1] *INCLUDE DummyQ25 DummyQ25HDFCIFIC
*IF [DummyQ25Axis=2 & NumberOfResponse[DummyQ25HDFCIFIC]=2] *INCLUDE DummyQ25 RandomValueOf[DummyQ25HDFCIFIC,1]

*IF [DummyQ25Axis!=2] *INCLUDE DummyQ25 DummyQ25HDFCIFIC


*QUESTION Q25Axis *RANK *MIN 5 *MAX 5 *ROT *IF [DummyQ25=2] 
[SHOW CARD]
Q25. What are the top 5 reasons for considering <b>AXIS Bank</b> for Credit card.
Pl rank them in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:Wide array of Credit card <b>benefits & offers</b> 
2:<b>Competitive interest</b> rate on late payments 
3:Getting <b>timely reminder</b> about payments & fees
4:Offers <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand has been very good
6:Heard <b>good reviews</b> about this credit card from others 
7:<b>Trustworthy</b> brand
8:</b>Using technology</b> to better engage with clients and deliver on their expectation
9:Have <b>useful digital solution/ Interface</b> for Credit card 
10:I have my <b>saving account</b> with this bank
11:The brand gives <b>grace period</b> on payment due date/<b>flexibility</b> on payment date.
12:Has best and relevant offers (for me) on <b>physical /instore shopping</b>.
13:Has best and relevant offers (for me) on <b>online shopping</b>.
14:Has best and relevant offers (for me) on <b>international hotels and flights</b>.
15:Has best and relevant offers (for me) on </b>domestic hotels and flights</b>.
16:Has best offers on <b>dining</b> which are relevant to me
17:Had best offers on <b>Airport lounge access</b> which are relevant to me
18:Has best offers for <b>entertainment /movies/concerts</b> etc
19:Has best <b>Rewards program</b>
20:Offers the best benefits for <b>Fuel expenses</b>
21:<b>Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
22:Provides waiver on Annual Fees / Lifetime free card
23:Provides better Value of <b>fees paid vs benefits earned</b>
24:Any other (Specify) *OPEN


*QUESTION Q25HDFC *RANK *MIN 5 *MAX 5 *ROT *IF [DummyQ25=10] 
[SHOW CARD]
Q25. What are the top 5 reasons for considering <b>HDFC Bank</b> for Credit card.
Pl rank them in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:Wide array of Credit card <b>benefits & offers</b> 
2:<b>Competitive interest</b> rate on late payments 
3:Getting <b>timely reminder</b> about payments & fees
4:Offers <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand has been very good
6:Heard <b>good reviews</b> about this credit card from others 
7:<b>Trustworthy</b> brand
8:</b>Using technology</b> to better engage with clients and deliver on their expectation
9:Have <b>useful digital solution/ Interface</b> for Credit card 
10:I have my <b>saving account</b> with this bank
11:The brand gives <b>grace period</b> on payment due date/<b>flexibility</b> on payment date.
12:Has best and relevant offers (for me) on <b>physical /instore shopping</b>.
13:Has best and relevant offers (for me) on <b>online shopping</b>.
14:Has best and relevant offers (for me) on <b>international hotels and flights</b>.
15:Has best and relevant offers (for me) on </b>domestic hotels and flights</b>.
16:Has best offers on <b>dining</b> which are relevant to me
17:Had best offers on <b>Airport lounge access</b> which are relevant to me
18:Has best offers for <b>entertainment /movies/concerts</b> etc
19:Has best <b>Rewards program</b>
20:Offers the best benefits for <b>Fuel expenses</b>
21:<b>Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
22:Provides waiver on Annual Fees / Lifetime free card
23:Provides better Value of <b>fees paid vs benefits earned</b>
24:Any other (Specify) *OPEN


*QUESTION Q25ICICI *RANK *MIN 5 *MAX 5 *ROT *IF [DummyQ25=12] 
[SHOW CARD]
Q25. What are the top 5 reasons for considering <b>ICICI Bank</b> for Credit card.
Pl rank them in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:Wide array of Credit card <b>benefits & offers</b> 
2:<b>Competitive interest</b> rate on late payments 
3:Getting <b>timely reminder</b> about payments & fees
4:Offers <b>higher credit limit</b>
5:<b>Previous associations</b> with this brand has been very good
6:Heard <b>good reviews</b> about this credit card from others 
7:<b>Trustworthy</b> brand
8:</b>Using technology</b> to better engage with clients and deliver on their expectation
9:Have <b>useful digital solution/ Interface</b> for Credit card 
10:I have my <b>saving account</b> with this bank
11:The brand gives <b>grace period</b> on payment due date/<b>flexibility</b> on payment date.
12:Has best and relevant offers (for me) on <b>physical /instore shopping</b>.
13:Has best and relevant offers (for me) on <b>online shopping</b>.
14:Has best and relevant offers (for me) on <b>international hotels and flights</b>.
15:Has best and relevant offers (for me) on </b>domestic hotels and flights</b>.
16:Has best offers on <b>dining</b> which are relevant to me
17:Had best offers on <b>Airport lounge access</b> which are relevant to me
18:Has best offers for <b>entertainment /movies/concerts</b> etc
19:Has best <b>Rewards program</b>
20:Offers the best benefits for <b>Fuel expenses</b>
21:<b>Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
22:Provides waiver on Annual Fees / Lifetime free card
23:Provides better Value of <b>fees paid vs benefits earned</b>
24:Any other (Specify) *OPEN


################################ Q26 New #################################

*QUESTION Q26 *RANK *MIN 5 *MAX 5 *ROT *IF [Q16bDummy!=2 & (Q17b2=1 | Q17b2=2)] 
[SHOW CARD]
Q26. You’ve mentioned that you consider Axis Bank for credit card in the future. However, you do not use it now. Can you tell me the reasons for considering but not using Axis Bank credit card? Please rank your Top 5 reasons in the order of importance. Where 1 is very important, 2 is 2nd most & so on..
1:Already <b>satisfied with my current credit card</b> provider. (not for FTI)
2:Didn't have a good experience with <b>other products of Axis Bank</b> (any product/service).
3:<b>Do not know much</b> about Axis Bank credit cards.
4:Did not get any positive <b>reviews or recommendation</b> from friends/family.
5:Do not have <b>competitive annual fees</b> or charges.
6:<b>Not available</b> or accessible in my location.
7:<b>Didn't meet eligibility</b> or approval requirements.
8:No clarity on how and where to initiate the <b>process of application</b> of Axis Bank Credit Cards
9:Tried to apply but the <b>processes were tedious</b>
10:<b>Limited offers or partnerships</b> with merchants.
11:<b>Benefits/rewards offered</b> are not as good as compared to other brands.
12:Does not have best and relevant offers (for me) on <b>international hotels and flights</b>.
13:Does not have best and relevant offers (for me) on <b>domestic hotels and flights</b>.
14:Does not have best offers on <b>dining</b> which are relevant to me
15:Does not have best offers on <b>Airport lounge access</b> which are relevant to me
16:Does not have best offers for <b>entertainment /movies/concerts</b> etc
17:<b>No Lifestyle offers</b> like Golf Access, Club Memberships, Experiences
18:Waiting for <b>better joining benefits</b> ( No Joining Fees, Extra Bonus points, promotional offers/vouchers etc)


##########################################################################
###########################     COMMUNICATION SECTION  ####################
##########################################################################


*QUESTION QC1 *MR *INCLUDE [Q13abc] *COLUMN 2 *RANDOM *IF [NumberOfResponse[Q13abc]>0]
[SHOW SCREEN TO THE RESPONDENT]
<b>ONLY CREDIT CARD ADS</b><br>
QC1. For which of these banks that offer credit card have you seen any advertisement in <b>last 1 month either online or offline</b>?  [MA]
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:None *NMUL *NOCON


*QUESTION QC2 *MR *INCLUDE [QC1] *COLUMN 2 *IF [NumberOfResponse[QC1]>0 & QC1!=99]
[SHOW SCREEN TO THE RESPONDENT]
<b>ONLY CREDIT CARD ADS</b><br>
QC2. And, for which all banks that offer credit card have you seen any Advertisements in <b>last 1 week either online or offline</b>? (MA)
*USELIST "Q13abcListImg"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
99:None *NMUL *NOCON


*QUESTION QC31 *MR *IF [QC2=1] *QROT
QC3.1 Where all have you seen or heard the advertisement in last 1 Week for <b>American Express Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC32 *MR *IF [QC2=2] *QROT
QC3.2 Where all have you seen or heard the advertisement in last 1 Week for <b>Axis Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC33 *MR *IF [QC2=3] *QROT
QC3.3 Where all have you seen or heard the advertisement in last 1 Week for <b>Bandhan Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC34 *MR *IF [QC2=4] *QROT
QC3.4 Where all have you seen or heard the advertisement in last 1 Week for <b>Bank of Baroda<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC35 *MR *IF [QC2=5] *QROT
QC3.5 Where all have you seen or heard the advertisement in last 1 Week for <b>Bank of India<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC36 *MR *IF [QC2=6] *QROT
QC3.6 Where all have you seen or heard the advertisement in last 1 Week for <b>Bank of Maharashtra<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC37 *MR *IF [QC2=7] *QROT
QC3.7 Where all have you seen or heard the advertisement in last 1 Week for <b>Canara Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC38 *MR *IF [QC2=8] *QROT
QC3.8 Where all have you seen or heard the advertisement in last 1 Week for <b>Citibank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC39 *MR *IF [QC2=9] *QROT
QC3.9 Where all have you seen or heard the advertisement in last 1 Week for <b>DBS<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC310 *MR *IF [QC2=10] *QROT
QC3.10 Where all have you seen or heard the advertisement in last 1 Week for <b>HDFC Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC311 *MR *IF [QC2=11] *QROT
QC3.11 Where all have you seen or heard the advertisement in last 1 Week for <b>HSBC<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC312 *MR *IF [QC2=12] *QROT
QC3.12 Where all have you seen or heard the advertisement in last 1 Week for <b>ICICI Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC313 *MR *IF [QC2=13] *QROT
QC3.13 Where all have you seen or heard the advertisement in last 1 Week for <b>IDFC First Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC314 *MR *IF [QC2=14] *QROT
QC3.14 Where all have you seen or heard the advertisement in last 1 Week for <b>IndusInd Bank<b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC315 *MR *IF [QC2=15] *QROT
QC3.15 Where all have you seen or heard the advertisement in last 1 Week for <b>Kotak Mahindra Bank</b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC316 *MR *IF [QC2=16] *QROT
QC3.16 Where all have you seen or heard the advertisement in last 1 Week for <b>Punjab National Bank</b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC317 *MR *IF [QC2=17] *QROT
QC3.17 Where all have you seen or heard the advertisement in last 1 Week for <b>Standard Chartered Bank</b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC318 *MR *IF [QC2=18] *QROT
QC3.18 Where all have you seen or heard the advertisement in last 1 Week for <b>State Bank of India / SBI</b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC319 *MR *IF [QC2=19] *QROT
QC3.19 Where all have you seen or heard the advertisement in last 1 Week for <b>YES Bank</b>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC395 *MR *IF [QC2=95] *QROT
QC3.95 Where all have you seen or heard the advertisement in last 1 Week for <font color="#FF73FF">{Q13a.95}</font>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC396 *MR *IF [QC2=96] *QROT
QC3.96 Where all have you seen or heard the advertisement in last 1 Week for <font color="#FF73FF">{Q13b.96}</font>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION QC397 *MR *IF [QC2=97] *QROT
QC3.97 Where all have you seen or heard the advertisement in last 1 Week for <font color="#FF73FF">{Q13b.97}</font>? [MA per row]
1:FM /Radio
2:Newspaper
3:Magazine
4:Television
5:Airport
6:Bank/ Branch
7:Digital Media / Internet
8:Outdoor (Billboard/banner/ poster) on highway/ roads
98:Others *OPEN

*QUESTION myDummy1 *INFO *DUMMY2
Just Dummy 1


*QUESTION QC41 *MR *IF [QC31=7] *QROT
QC4.1 Where on digital/ internet have you seen the ad <b>in Last 1 Week forAmerican Express Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC42 *MR *IF [QC32=7] *QROT
QC4.2 Where on digital/ internet have you seen the ad <b>in Last 1 Week forAxis Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC43 *MR *IF [QC33=7] *QROT
QC4.3 Where on digital/ internet have you seen the ad <b>in Last 1 Week forBandhan Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC44 *MR *IF [QC34=7] *QROT
QC4.4 Where on digital/ internet have you seen the ad <b>in Last 1 Week forBank of Baroda<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC45 *MR *IF [QC35=7] *QROT
QC4.5 Where on digital/ internet have you seen the ad <b>in Last 1 Week forBank of India<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC46 *MR *IF [QC36=7] *QROT
QC4.6 Where on digital/ internet have you seen the ad <b>in Last 1 Week forBank of Maharashtra<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC47 *MR *IF [QC37=7] *QROT
QC4.7 Where on digital/ internet have you seen the ad <b>in Last 1 Week forCanara Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC48 *MR *IF [QC38=7] *QROT
QC4.8 Where on digital/ internet have you seen the ad <b>in Last 1 Week forCitibank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC49 *MR *IF [QC39=7] *QROT
QC4.9 Where on digital/ internet have you seen the ad <b>in Last 1 Week forDBS<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC410 *MR *IF [QC310=7] *QROT
QC4.10 Where on digital/ internet have you seen the ad <b>in Last 1 Week forHDFC Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC411 *MR *IF [QC311=7] *QROT
QC4.11 Where on digital/ internet have you seen the ad <b>in Last 1 Week forHSBC<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC412 *MR *IF [QC312=7] *QROT
QC4.12 Where on digital/ internet have you seen the ad <b>in Last 1 Week forICICI Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC413 *MR *IF [QC313=7] *QROT
QC4.13 Where on digital/ internet have you seen the ad <b>in Last 1 Week forIDFC First Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC414 *MR *IF [QC314=7] *QROT
QC4.14 Where on digital/ internet have you seen the ad <b>in Last 1 Week forIndusInd Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC415 *MR *IF [QC315=7] *QROT
QC4.15 Where on digital/ internet have you seen the ad <b>in Last 1 Week forKotak Mahindra Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC416 *MR *IF [QC316=7] *QROT
QC4.16 Where on digital/ internet have you seen the ad <b>in Last 1 Week forPunjab National Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC417 *MR *IF [QC317=7] *QROT
QC4.17 Where on digital/ internet have you seen the ad <b>in Last 1 Week forStandard Chartered Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC418 *MR *IF [QC318=7] *QROT
QC4.18 Where on digital/ internet have you seen the ad <b>in Last 1 Week forState Bank of India / SBI<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC419 *MR *IF [QC319=7] *QROT
QC4.19 Where on digital/ internet have you seen the ad <b>in Last 1 Week forYES Bank<b>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC495 *MR *IF [QC395=7] *QROT
QC4.95 Where on digital/ internet have you seen the ad <b>in Last 1 Week for <font color="#FF73FF">{Q13a.95}</font>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC496 *MR *IF [QC396=7] *QROT
QC4.96 Where on digital/ internet have you seen the ad <b>in Last 1 Week for <font color="#FF73FF">{Q13b.96}</font>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

*QUESTION QC497 *MR *IF [QC397=7] *QROT
QC4.97 Where on digital/ internet have you seen the ad <b>in Last 1 Week for <font color="#FF73FF">{Q13b.97}</font>? [MA per row]
1:YouTube
2:Facebook
3:Instagram
4:LinkedIn
5:<b>Credit Card</b> Brand websites
6:Other Financial/ Payment website/ app
7:Searching on Google/Internet
8:X/Twitter

#########################COMMUNICATON EVALUATION ######################
*STARTREC "QC5abc"

*QUESTION QC5a1 *OPEN *DKCS "Can't Say" "999"
TO BE CAPTURED IN OE SHEET
PROBE IN DETAIL AND TAKE THE RESPONSES SEPARATELY
<br>QC5a.1 Can you describe any advertisements about credit card which you would have seen recently on TV or any digital medium? Please describe whatever you remember about the ad, any people, dialogue, scene etc. Please tell in detail.

*IF [QC5a1=999] *GOTO myAdNum

*QUESTION QC5b1 *OPEN *IF [QC5a1!=999]
TO BE CAPTURED IN OE SHEET<br>
QC5b.1 Please tell me according to you, what was the key message take away from the advertisement that was shown/heard by you?
<br>PROBE What else do you think? Anything else? [MA]

*QUESTION QC5c1 *SR *INCLUDE [Q13abc] *RANDOM *IF [QC5a1!=999 & NumberOfResponse[Q13abc]>0]
QC5c.1 For which bank that offers credit card this ad was for which you just described. SA
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
98:Others *OPEN *NOCON
99:Don't know/ Can't remember *NMUL *NOCON

#*QUESTION QC5d1 *SR *IF [QC5a1!=999]
#QC5d.1 Do you remember any other ad for credit cards?  SA
#1:Yes
#2:No

########################################### 2nd spont ad

*QUESTION QC5a2 *OPEN *DKCS "Can't Say" "999"
TO BE CAPTURED IN OE SHEET
PROBE IN DETAIL AND TAKE THE RESPONSES SEPARATELY
<br>QC5a.2 Can you describe any others advertisements about credit card which you would have seen recently on TV or any digital medium? Please describe whatever you remember about the ad, any people, dialogue, scene etc. Please tell in detail.

*IF [QC5a2=999] *GOTO myAdNum

*QUESTION QC5b2 *OPEN *IF [QC5a2!=999]
TO BE CAPTURED IN OE SHEET<br>
QC5b.2 Please tell me according to you, what was the key message take away from the advertisement that was shown/heard by you?
<br>PROBE What else do you think? Anything else? [MA]

*QUESTION QC5c2 *SR *INCLUDE [Q13abc] *RANDOM *IF [QC5a2!=999 & NumberOfResponse[Q13abc]>0]
QC5c.2 For which bank that offers credit card this ad was for which you just described. SA
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
98:Others *OPEN *NOCON
99:Don't know/ Can't remember *NMUL *NOCON

#*QUESTION QC5d2 *SR *IF [QC5d1=1 & QC5a2!=999]
#QC5d.2 Do you remember any other ad for credit cards?  SA
#1:Yes
#2:No
#
#*IF [QC5d2=2] *GOTO QP1Txt

########################################### 3rd spont ad

*QUESTION QC5a3 *OPEN *DKCS "Can't Say" "999"
TO BE CAPTURED IN OE SHEET
PROBE IN DETAIL AND TAKE THE RESPONSES SEPARATELY
<br>QC5a.3 Can you describe any others advertisements about credit card which you would have seen recently on TV or any digital medium? Please describe whatever you remember about the ad, any people, dialogue, scene etc. Please tell in detail.

*QUESTION QC5b3 *OPEN *IF [QC5a3!=999]
TO BE CAPTURED IN OE SHEET<br>
QC5b.3 Please tell me according to you, what was the key message take away from the advertisement that was shown/heard by you?
<br>PROBE What else do you think? Anything else? [MA]

*QUESTION QC5c3 *SR *INCLUDE [Q13abc] *RANDOM *IF [QC5a3!=999 & NumberOfResponse[Q13abc]>0]
QC5c.3 For which bank that offers credit card this ad was for which you just described. SA
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
98:Others *OPEN *NOCON
99:Don't know/ Can't remember *NMUL *NOCON

*ENDREC



###################################################################################
################################Ad Section (Video) ##############################
####################################################################################

*QUESTION myAdNum *SR *DUMMY2
Random Number
1:1
2:2
3:3

######################### Ad Evaluation Section #####################
#####################################################################
######################## Ad rotation for 1st Ad #####################

*QUESTION AdInfo *INFO
<b>[INTERVIEWER INSTRUCTION]</b>
<br>Show Ad to respondent upto satisfaction then take back the stimulus and ask questions.<br>
<br><b>[INTERVIEWER TO SAY]</b>
<br>Now we will show you some advertisements which were aired recently. Please observe carefully. We would need your honest and valuable feedback. 

*QUESTION EnTvAd1 *MEDIA *VIDEO "Ad0241.mp4"
Please click the play button to watch the ad

*QUESTION QC6aTvAd1 *SR
QC6a. Now that we have shown you an advertisement that have been on Internet and social media recently, I would like you to tell me if you have seen this ad before or not. Which of these statements describes the best? [SA]
1:Definitely haven't seen it
2:Don't think I've seen it
3:Unsure whether I've seen it
4:Yes I think I've seen it
5:Yes definitely seen it
9:Don't know (DO NOT SHOW/READ)            

*QUESTION QC6bTvAd1 *SR *INCLUDE [Q13abc]
<b>[DO NOT PROBE]</b><br>
QC6b. Now please tell me for which banks that offer credit card the advertisements was for?  [SA]
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
98:Others (Please specify) *OPEN
99:Don't know/ Don't remember *NMUL *NOCON

*QUESTION QC7TvAd1 *SR
<b>[Show Screen]</b><br>
QC7. Which of these statements best describe your overall feeling about this advertisement? Would you say you
1:Dislike it a lot              	
2:Dislike it a little              	
3:Neither like nor dislike                	
4:Like it a little                	
5:Like it very much                  	
9:Don't Know (DO NOT SHOW/READ)

*QUESTION QC8TvAd1 *SR
<b>[Show Screen]</b><br>
QC8. To what extent do you find the claims made in the ad believable
1:Not at all believable   	
2:Partially believable   	
3:Mostly believable 	
4:Completely believable

*QUESTION QC9TvAd1 *SR
<b>[Show Screen]</b><br>
QC9. Which of these statements come closest to the way the ad left you feeling about the brand? [SA]
1:Will definitely not apply for this credit card
2:Will probably not apply for this credit card
3:May or may not apply for this credit card
4:Will probably apply for this credit card
5:Will definitely apply for this credit card

*QUESTION Q10C1TvAd1 *SR *QROT
<b>[Show Screen]</b><br>
QC10. Now I will read out a statement from the ad for you. Please tell how much you associate this message coming from this Ad, where 1 means you Do not associate at all with the ad and 5 means you Associate a lot with the ad.
<br><b><font color="#FF73FF">Users can earn reward points on everyday spends such as shopping, dining, and travel.</font><b>
1:Do not associate at all (1)
2:2
3:3
4:4
5:Very much associate (5)

*QUESTION Q10C2TvAd1 *SR *QROT
<b>[Show Screen]</b><br>
QC10. Now I will read out a statement from the ad for you. Please tell how much you associate this message coming from this Ad, where 1 means you Do not associate at all with the ad and 5 means you Associate a lot with the ad.
<br><b><font color="#FF73FF">The rewards from this card can be used for travel experiences such as flights.</font><b>
1:Do not associate at all (1)
2:2
3:3
4:4
5:Very much associate (5)

*QUESTION Q10C3TvAd1 *SR *QROT
<b>[Show Screen]</b><br>
QC10. Now I will read out a statement from the ad for you. Please tell how much you associate this message coming from this Ad, where 1 means you Do not associate at all with the ad and 5 means you Associate a lot with the ad.
<br><b><font color="#FF73FF">This card offers valuable rewards for regular travellers.</font><b>
1:Do not associate at all (1)
2:2
3:3
4:4
5:Very much associate (5)

*QUESTION Q11CTvAd1 *MR *RANDOM
QC11. Imagine that you are watching this ad on TV in your home, what would have been your action after watching it?  
Please choose all that applies
1:The ad would have not changed my perception on the brand in any way *NMUL
2:The ad would have made me feel like checking the website or app of the brand
3:The ad would have made me feel like checking more about brand and its services on the internet / google 
4:The ad would have made me feel like considering it in the near future.
5:The ad would have made me feel like subscribing or using it for my immediate credit Card needs
6:The ad would have made feel like calling up call center to know more about this credit card and its offerings

################################### Still Ad 2 ###################################

*QUESTION myStAdNum *SR *DUMMY2
Random Number
1:1
2:2

*INCLUDE myStAdNum RanBetweenOf[(1;2),1]

*QUESTION AdSt1st *SR *DUMMY2 *INRLD
1st Still Ad
1:Ad1241.jpg
2:Ad1242.jpg

*IF [myStAdNum=1] *INCLUDE AdSt1st [1]
*IF [myStAdNum=2] *INCLUDE AdSt1st [2]

*QUESTION StAdInfo1 *IMAGE *PICT "{AdSt1st}"
INTERVIEWER TO SAY
Now we will show you some advertisements which were displayed recently. Please observe carefully. We would need your honest and valuable feedback. 

*LIST "QC6aStLIST"
1:Definitely haven't seen it
2:Don't think I've seen it
3:Unsure whether I've seen it
4:Yes I think I've seen it
5:Yes definitely seen it
9:Don't know (DO NOT SHOW/READ)

*QUESTION QC6aStAd1 *SR
QCS6a. Now that we have shown you an advertisement that was displayed recently, I would like you to tell me if you have seen this ad before or not. Which of these statements describes the best? [SA] 
*USELIST "QC6aStLIST"

*QUESTION QC6bStAd1 *SR *INCLUDE [Q13abc]
<b>[DO NOT PROBE]</b><br>
QCS6b. Now can you please tell me the brand for which the advertisement was for?  SA
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
98:Others (Please specify) *OPEN
99:Don't know/ Don't remember *NMUL *NOCON

*LIST "QCS6cStList"
1:Social Media (Instagram/ Linkedin/ Facebook etc.)
2:Print ads in newspapers/magazines
3:Outdoor – Billboard / Hoardings
4:Outdoor – Bus shelter
5:Don't know/ Don't remember *NMUL
6:Others (Pl specify) *OPEN

*QUESTION QCS6cStAd1 *SR *ROT
QCS6c. Where have you seen this ad?  SA
*USELIST "QCS6cStList"


################################### Still Ad 2 ###################################

*QUESTION AdSt2nd *SR *DUMMY2 *INRLD
2nd Still Ad
1:Ad1241.jpg
2:Ad1242.jpg

*IF [AdSt1st=1] *INCLUDE AdSt2nd [2]
*IF [AdSt1st=2] *INCLUDE AdSt2nd [1]

*QUESTION StAdInfo2 *IMAGE *PICT "{AdSt2nd}"
INTERVIEWER TO SAY
Now we will show you some advertisements which were displayed recently. Please observe carefully. We would need your honest and valuable feedback. 

*QUESTION QC6aStAd2 *SR
QCS6a. Now that we have shown you an advertisement that was displayed recently, I would like you to tell me if you have seen this ad before or not. Which of these statements describes the best? [SA] 
*USELIST "QC6aStLIST"

*QUESTION QC6bStAd2 *SR *INCLUDE [Q13abc]
<b>[DO NOT PROBE]</b><br>
QCS6b. Now can you please tell me the brand for which the advertisement was for?  SA
*USELIST "Q13abcList"
95:{Q13a.95}
96:{Q13b.96}
#97:{Q13b.97}
98:Others (Please specify) *OPEN
99:Don't know/ Don't remember *NMUL *NOCON

*QUESTION QCS6cStAd2 *SR *ROT
QCS6c. Where have you seen this ad?  SA
*USELIST "QCS6cStList"

########################################### Comparison ########################

*QUESTION QCS7St *SR
QCS7. Which of these statements best describe your overall feeling about both these advertisements? Would you say you ... [SA]
1:Dislike it a lot              	
2:Dislike it a little              	
3:Neither like nor dislike                	
4:Like it a little                	
5:Like it very much                  	
9:Don't Know (DO NOT SHOW/READ)   	

*QUESTION QCS8St *SR
SHOW SCREEN - BELIEVABILITY ALL
QCS8. To what extent do you find the claims made in both ads believable [SA]
1:Not at all believable   	
2:Partially believable   	
3:Mostly believable 	
4:Completely believable 	

*QUESTION QCS9St *SR
QCS9. Which of these statements come closest to the way the ad left you feeling about the brand? Please choose all that apply. [SA]
1:Will definitely not apply for this credit card	
2:Will probably not apply for this credit card	
3:May or may not apply for this credit card	
4:Will probably apply for this credit card	
5:Will definitely apply for this credit card	


*QUESTION QCS10St1 *SR *QROT
<b>[Show Screen]</b><br>
QCS10. Now I will read out a statement from the ads for you. Please tell how much you associate this message coming from these Ads, where 1 means you Do not associate at all with the ads and 5 means you Associate a lot with the ads.
<br><b><font color="#FF73FF">Users can earn reward points on everyday spends such as shopping, dining, and travel.</font><b>
1:Do not associate at all (1)
2:2
3:3
4:4
5:Very much associate (5)

*QUESTION QCS10St2 *SR *QROT
<b>[Show Screen]</b><br>
QCS10. Now I will read out a statement from the ads for you. Please tell how much you associate this message coming from these Ads, where 1 means you Do not associate at all with the ads and 5 means you Associate a lot with the ads.
<br><b><font color="#FF73FF">The rewards from this card can be used for travel experiences such as flights.</font><b>
1:Do not associate at all (1)
2:2
3:3
4:4
5:Very much associate (5)


###################################################################################
################################CARD PROFILING SECTION##############################
####################################################################################

*QUESTION QP1Txt *SR *DUMMY2
QLabel for QP1
1:QP1. To whom all do you reach out for in information/ update regarding your credit cards.? Please tell the Top 3 sources
2:QP1. As you are looking for a credit card, where did you search for different cards to make your selection? Please tell the Top 3 sources

*IF [QHD12d=1 | QHD12d=2] *INCLUDE QP1Txt [1] 
*IF [QHD12d=3] *INCLUDE QP1Txt [2] 

#*NOBACKBTN
#*IF [QHD12d!=4]

*QUESTION QP1 *RANK *MIN 3 *MAX 3 
[SHOW SCREEN]<br>
{QP1Txt}
1:Search on Google / internet
2:Friends / family /relative 
3:Advertisement on Social Media (Instagram/ Linkedin/ Facebook etc.)
4:Seen ad in newspapers/magazines
5:Heard an ad on Radio/ FM
6:Seen ad on a property website like Bankbazaar, India lends etc
7:Seen ad on hoardings/ outdoors
8:Got promotional mail / SMS / call from banks/ financial institution
9:Branch / ATM
98:Others please specify *OPEN

*QUESTION QP1Mr *MR *DUMMY2
[SHOW SCREEN]<br>
QP1. As you are looking for a credit card, where did you search for different cards to make your selection? Please tell the Top 3 sources (MA)
1:Search on Google / internet
2:Friends / family /relative 
3:Advertisement on Social Media (Instagram/ Linkedin/ Facebook etc.)
4:Seen ad in newspapers/magazines
5:Heard an ad on Radio/ FM
6:Seen ad on a property website like Bankbazaar, India lends etc
7:Seen ad on hoardings/ outdoors
8:Got promotional mail / SMS / call from banks/ financial institution
9:Branch / ATM
98:Others please specify *OPEN

*IF [ValueOf[QP1.1]>0] *INCLUDE QP1Mr [1]
*IF [ValueOf[QP1.2]>0] *INCLUDE QP1Mr [2]
*IF [ValueOf[QP1.3]>0] *INCLUDE QP1Mr [3]
*IF [ValueOf[QP1.4]>0] *INCLUDE QP1Mr [4]
*IF [ValueOf[QP1.5]>0] *INCLUDE QP1Mr [5]
*IF [ValueOf[QP1.6]>0] *INCLUDE QP1Mr [6]
*IF [ValueOf[QP1.7]>0] *INCLUDE QP1Mr [7]
*IF [ValueOf[QP1.8]>0] *INCLUDE QP1Mr [8]
*IF [ValueOf[QP1.9]>0] *INCLUDE QP1Mr [9]
*IF [ValueOf[QP1.98]>0] *INCLUDE QP1Mr [98]


*STARTREC "QP2abcd"

*GRIDLIST "GridQP2a"
1:Cash
2:UPI like Google Pay, BHIM UPI, PhonePe etc.
3:Debit Card
4:Credit Card
5:E-Wallet/Digital wallet like Paytm wallet, Airtel wallet etc.
6:Net banking/ NEFT
7:UPI on credit card


*QUESTION QP2a *GRIDSR *USEGRIDLIST "GridQP2a"
QP2a. Now I want to think of your last 5 payments made <b>Upto Rs. 1000 for any purpose</b>. Can you please tell us the mode of payment made for each of them. SA FOR EACH  PAYMENT
1:Last payments
2:2nd Last payments
3:3rd Last payments
4:4th Last payments
5:5th Last payments

*QUESTION QP2b *GRIDSR *USEGRIDLIST "GridQP2a"
QP2b. Similarly, now think of last 5  payments made between <b>Rs. 1001 to Rs. 2000 for any purpose</b>. Can you please tell us the mode of payment made for each of them. SA FOR EACH  PAYMENT
1:Last payments
2:2nd Last payments
3:3rd Last payments
4:4th Last payments
5:5th Last payments

*QUESTION QP2c *GRIDSR *USEGRIDLIST "GridQP2a"
QP2c. Similarly, now think of last 5  payments made between <b>Rs. 2001 to Rs. 5000 for any purpose</b>. Can you please tell us the mode of payment made for each of them. SA FOR EACH  PAYMENT
1:Last payments
2:2nd Last payments
3:3rd Last payments
4:4th Last payments
5:5th Last payments

*QUESTION QP2d *GRIDSR *USEGRIDLIST "GridQP2a"
QP2d. And now think of your last 5  payments made for <b>more than Rs 5000/- for any purpose</b>. Can you please tell us the mode of payment made for each of them. SA FOR EACH  PAYMENT
1:Last payments
2:2nd Last payments
3:3rd Last payments
4:4th Last payments
5:5th Last payments


*ENDREC


*IF [QHD12d=3 | QHD12d=4 | NumberOfResponse[CardBrand]=0] *GOTO QM1

*GRIDLIST "GridQP3a"
1:It's a free a card
2:It's a paid card but fee gets waived off post a minimum expenditure
3:It's a Paid card

# *SHOWASFORM

*QUESTION QP3a *GRIDSR *USEGRIDLIST "GridQP3a" *INCLUDE [CardBrand] *IMGADJBY 400
QP3a. Can you tell which of these cards are free & for which you pay an annual fee?
*USELIST "CCListImage"


*QUESTION DummyQP3b *MR *DUMMY2
QP3a. Can you tell which of these cards are free & for which you pay an annual fee?
*USELIST "CCList"


*IF [ValueOf[QP3a.1]=2 | ValueOf[QP3a.1]=3] *INCLUDE DummyQP3b [1]
*IF [ValueOf[QP3a.2]=2 | ValueOf[QP3a.2]=3] *INCLUDE DummyQP3b [2]
*IF [ValueOf[QP3a.3]=2 | ValueOf[QP3a.3]=3] *INCLUDE DummyQP3b [3]
*IF [ValueOf[QP3a.4]=2 | ValueOf[QP3a.4]=3] *INCLUDE DummyQP3b [4]
*IF [ValueOf[QP3a.5]=2 | ValueOf[QP3a.5]=3] *INCLUDE DummyQP3b [5]
*IF [ValueOf[QP3a.6]=2 | ValueOf[QP3a.6]=3] *INCLUDE DummyQP3b [6]
*IF [ValueOf[QP3a.7]=2 | ValueOf[QP3a.7]=3] *INCLUDE DummyQP3b [7]
*IF [ValueOf[QP3a.8]=2 | ValueOf[QP3a.8]=3] *INCLUDE DummyQP3b [8]
*IF [ValueOf[QP3a.9]=2 | ValueOf[QP3a.9]=3] *INCLUDE DummyQP3b [9]
*IF [ValueOf[QP3a.10]=2 | ValueOf[QP3a.10]=3] *INCLUDE DummyQP3b [10]
*IF [ValueOf[QP3a.11]=2 | ValueOf[QP3a.11]=3] *INCLUDE DummyQP3b [11]
*IF [ValueOf[QP3a.12]=2 | ValueOf[QP3a.12]=3] *INCLUDE DummyQP3b [12]
*IF [ValueOf[QP3a.13]=2 | ValueOf[QP3a.13]=3] *INCLUDE DummyQP3b [13]
*IF [ValueOf[QP3a.14]=2 | ValueOf[QP3a.14]=3] *INCLUDE DummyQP3b [14]
*IF [ValueOf[QP3a.15]=2 | ValueOf[QP3a.15]=3] *INCLUDE DummyQP3b [15]
*IF [ValueOf[QP3a.16]=2 | ValueOf[QP3a.16]=3] *INCLUDE DummyQP3b [16]
*IF [ValueOf[QP3a.17]=2 | ValueOf[QP3a.17]=3] *INCLUDE DummyQP3b [17]
*IF [ValueOf[QP3a.18]=2 | ValueOf[QP3a.18]=3] *INCLUDE DummyQP3b [18]
*IF [ValueOf[QP3a.19]=2 | ValueOf[QP3a.19]=3] *INCLUDE DummyQP3b [19]
*IF [ValueOf[QP3a.20]=2 | ValueOf[QP3a.20]=3] *INCLUDE DummyQP3b [20]
*IF [ValueOf[QP3a.21]=2 | ValueOf[QP3a.21]=3] *INCLUDE DummyQP3b [21]
*IF [ValueOf[QP3a.22]=2 | ValueOf[QP3a.22]=3] *INCLUDE DummyQP3b [22]
*IF [ValueOf[QP3a.23]=2 | ValueOf[QP3a.23]=3] *INCLUDE DummyQP3b [23]
*IF [ValueOf[QP3a.24]=2 | ValueOf[QP3a.24]=3] *INCLUDE DummyQP3b [24]
*IF [ValueOf[QP3a.25]=2 | ValueOf[QP3a.25]=3] *INCLUDE DummyQP3b [25]
*IF [ValueOf[QP3a.26]=2 | ValueOf[QP3a.26]=3] *INCLUDE DummyQP3b [26]
*IF [ValueOf[QP3a.27]=2 | ValueOf[QP3a.27]=3] *INCLUDE DummyQP3b [27]
*IF [ValueOf[QP3a.28]=2 | ValueOf[QP3a.28]=3] *INCLUDE DummyQP3b [28]
*IF [ValueOf[QP3a.29]=2 | ValueOf[QP3a.29]=3] *INCLUDE DummyQP3b [29]
*IF [ValueOf[QP3a.30]=2 | ValueOf[QP3a.30]=3] *INCLUDE DummyQP3b [30]
*IF [ValueOf[QP3a.31]=2 | ValueOf[QP3a.31]=3] *INCLUDE DummyQP3b [31]
*IF [ValueOf[QP3a.32]=2 | ValueOf[QP3a.32]=3] *INCLUDE DummyQP3b [32]
*IF [ValueOf[QP3a.33]=2 | ValueOf[QP3a.33]=3] *INCLUDE DummyQP3b [33]
*IF [ValueOf[QP3a.34]=2 | ValueOf[QP3a.34]=3] *INCLUDE DummyQP3b [34]
*IF [ValueOf[QP3a.35]=2 | ValueOf[QP3a.35]=3] *INCLUDE DummyQP3b [35]
*IF [ValueOf[QP3a.36]=2 | ValueOf[QP3a.36]=3] *INCLUDE DummyQP3b [36]
*IF [ValueOf[QP3a.1198]=2 | ValueOf[QP3a.1198]=3] *INCLUDE DummyQP3b [1198]
*IF [ValueOf[QP3a.37]=2 | ValueOf[QP3a.37]=3] *INCLUDE DummyQP3b [37]
*IF [ValueOf[QP3a.38]=2 | ValueOf[QP3a.38]=3] *INCLUDE DummyQP3b [38]
*IF [ValueOf[QP3a.39]=2 | ValueOf[QP3a.39]=3] *INCLUDE DummyQP3b [39]
*IF [ValueOf[QP3a.40]=2 | ValueOf[QP3a.40]=3] *INCLUDE DummyQP3b [40]
*IF [ValueOf[QP3a.41]=2 | ValueOf[QP3a.41]=3] *INCLUDE DummyQP3b [41]
*IF [ValueOf[QP3a.42]=2 | ValueOf[QP3a.42]=3] *INCLUDE DummyQP3b [42]
*IF [ValueOf[QP3a.43]=2 | ValueOf[QP3a.43]=3] *INCLUDE DummyQP3b [43]
*IF [ValueOf[QP3a.44]=2 | ValueOf[QP3a.44]=3] *INCLUDE DummyQP3b [44]
*IF [ValueOf[QP3a.45]=2 | ValueOf[QP3a.45]=3] *INCLUDE DummyQP3b [45]
*IF [ValueOf[QP3a.46]=2 | ValueOf[QP3a.46]=3] *INCLUDE DummyQP3b [46]
*IF [ValueOf[QP3a.47]=2 | ValueOf[QP3a.47]=3] *INCLUDE DummyQP3b [47]
*IF [ValueOf[QP3a.48]=2 | ValueOf[QP3a.48]=3] *INCLUDE DummyQP3b [48]
*IF [ValueOf[QP3a.49]=2 | ValueOf[QP3a.49]=3] *INCLUDE DummyQP3b [49]
*IF [ValueOf[QP3a.50]=2 | ValueOf[QP3a.50]=3] *INCLUDE DummyQP3b [50]
*IF [ValueOf[QP3a.51]=2 | ValueOf[QP3a.51]=3] *INCLUDE DummyQP3b [51]
*IF [ValueOf[QP3a.52]=2 | ValueOf[QP3a.52]=3] *INCLUDE DummyQP3b [52]
*IF [ValueOf[QP3a.53]=2 | ValueOf[QP3a.53]=3] *INCLUDE DummyQP3b [53]
*IF [ValueOf[QP3a.54]=2 | ValueOf[QP3a.54]=3] *INCLUDE DummyQP3b [54]
*IF [ValueOf[QP3a.55]=2 | ValueOf[QP3a.55]=3] *INCLUDE DummyQP3b [55]
*IF [ValueOf[QP3a.56]=2 | ValueOf[QP3a.56]=3] *INCLUDE DummyQP3b [56]
*IF [ValueOf[QP3a.57]=2 | ValueOf[QP3a.57]=3] *INCLUDE DummyQP3b [57]
*IF [ValueOf[QP3a.58]=2 | ValueOf[QP3a.58]=3] *INCLUDE DummyQP3b [58]
*IF [ValueOf[QP3a.59]=2 | ValueOf[QP3a.59]=3] *INCLUDE DummyQP3b [59]
*IF [ValueOf[QP3a.60]=2 | ValueOf[QP3a.60]=3] *INCLUDE DummyQP3b [60]
*IF [ValueOf[QP3a.61]=2 | ValueOf[QP3a.61]=3] *INCLUDE DummyQP3b [61]
*IF [ValueOf[QP3a.62]=2 | ValueOf[QP3a.62]=3] *INCLUDE DummyQP3b [62]
*IF [ValueOf[QP3a.63]=2 | ValueOf[QP3a.63]=3] *INCLUDE DummyQP3b [63]
*IF [ValueOf[QP3a.64]=2 | ValueOf[QP3a.64]=3] *INCLUDE DummyQP3b [64]
*IF [ValueOf[QP3a.65]=2 | ValueOf[QP3a.65]=3] *INCLUDE DummyQP3b [65]
*IF [ValueOf[QP3a.66]=2 | ValueOf[QP3a.66]=3] *INCLUDE DummyQP3b [66]
*IF [ValueOf[QP3a.67]=2 | ValueOf[QP3a.67]=3] *INCLUDE DummyQP3b [67]
*IF [ValueOf[QP3a.68]=2 | ValueOf[QP3a.68]=3] *INCLUDE DummyQP3b [68]
*IF [ValueOf[QP3a.69]=2 | ValueOf[QP3a.69]=3] *INCLUDE DummyQP3b [69]
*IF [ValueOf[QP3a.70]=2 | ValueOf[QP3a.70]=3] *INCLUDE DummyQP3b [70]
*IF [ValueOf[QP3a.71]=2 | ValueOf[QP3a.71]=3] *INCLUDE DummyQP3b [71]
*IF [ValueOf[QP3a.72]=2 | ValueOf[QP3a.72]=3] *INCLUDE DummyQP3b [72]
*IF [ValueOf[QP3a.73]=2 | ValueOf[QP3a.73]=3] *INCLUDE DummyQP3b [73]
*IF [ValueOf[QP3a.74]=2 | ValueOf[QP3a.74]=3] *INCLUDE DummyQP3b [74]
*IF [ValueOf[QP3a.75]=2 | ValueOf[QP3a.75]=3] *INCLUDE DummyQP3b [75]
*IF [ValueOf[QP3a.76]=2 | ValueOf[QP3a.76]=3] *INCLUDE DummyQP3b [76]
*IF [ValueOf[QP3a.77]=2 | ValueOf[QP3a.77]=3] *INCLUDE DummyQP3b [77]
*IF [ValueOf[QP3a.29]=2 | ValueOf[QP3a.29]=3] *INCLUDE DummyQP3b [29]
*IF [ValueOf[QP3a.78]=2 | ValueOf[QP3a.78]=3] *INCLUDE DummyQP3b [78]
*IF [ValueOf[QP3a.79]=2 | ValueOf[QP3a.79]=3] *INCLUDE DummyQP3b [79]
*IF [ValueOf[QP3a.80]=2 | ValueOf[QP3a.80]=3] *INCLUDE DummyQP3b [80]
*IF [ValueOf[QP3a.81]=2 | ValueOf[QP3a.81]=3] *INCLUDE DummyQP3b [81]
*IF [ValueOf[QP3a.82]=2 | ValueOf[QP3a.82]=3] *INCLUDE DummyQP3b [82]
*IF [ValueOf[QP3a.83]=2 | ValueOf[QP3a.83]=3] *INCLUDE DummyQP3b [83]
*IF [ValueOf[QP3a.84]=2 | ValueOf[QP3a.84]=3] *INCLUDE DummyQP3b [84]
*IF [ValueOf[QP3a.85]=2 | ValueOf[QP3a.85]=3] *INCLUDE DummyQP3b [85]
*IF [ValueOf[QP3a.86]=2 | ValueOf[QP3a.86]=3] *INCLUDE DummyQP3b [86]
*IF [ValueOf[QP3a.87]=2 | ValueOf[QP3a.87]=3] *INCLUDE DummyQP3b [87]
*IF [ValueOf[QP3a.88]=2 | ValueOf[QP3a.88]=3] *INCLUDE DummyQP3b [88]
*IF [ValueOf[QP3a.89]=2 | ValueOf[QP3a.89]=3] *INCLUDE DummyQP3b [89]
*IF [ValueOf[QP3a.90]=2 | ValueOf[QP3a.90]=3] *INCLUDE DummyQP3b [90]
*IF [ValueOf[QP3a.91]=2 | ValueOf[QP3a.91]=3] *INCLUDE DummyQP3b [91]
*IF [ValueOf[QP3a.92]=2 | ValueOf[QP3a.92]=3] *INCLUDE DummyQP3b [92]
*IF [ValueOf[QP3a.93]=2 | ValueOf[QP3a.93]=3] *INCLUDE DummyQP3b [93]
*IF [ValueOf[QP3a.94]=2 | ValueOf[QP3a.94]=3] *INCLUDE DummyQP3b [94]
*IF [ValueOf[QP3a.95]=2 | ValueOf[QP3a.95]=3] *INCLUDE DummyQP3b [95]
*IF [ValueOf[QP3a.96]=2 | ValueOf[QP3a.96]=3] *INCLUDE DummyQP3b [96]
*IF [ValueOf[QP3a.97]=2 | ValueOf[QP3a.97]=3] *INCLUDE DummyQP3b [97]
*IF [ValueOf[QP3a.98]=2 | ValueOf[QP3a.98]=3] *INCLUDE DummyQP3b [98]
*IF [ValueOf[QP3a.99]=2 | ValueOf[QP3a.99]=3] *INCLUDE DummyQP3b [99]
*IF [ValueOf[QP3a.100]=2 | ValueOf[QP3a.100]=3] *INCLUDE DummyQP3b [100]
*IF [ValueOf[QP3a.101]=2 | ValueOf[QP3a.101]=3] *INCLUDE DummyQP3b [101]
*IF [ValueOf[QP3a.102]=2 | ValueOf[QP3a.102]=3] *INCLUDE DummyQP3b [102]
*IF [ValueOf[QP3a.103]=2 | ValueOf[QP3a.103]=3] *INCLUDE DummyQP3b [103]
*IF [ValueOf[QP3a.104]=2 | ValueOf[QP3a.104]=3] *INCLUDE DummyQP3b [104]
*IF [ValueOf[QP3a.105]=2 | ValueOf[QP3a.105]=3] *INCLUDE DummyQP3b [105]
*IF [ValueOf[QP3a.106]=2 | ValueOf[QP3a.106]=3] *INCLUDE DummyQP3b [106]
*IF [ValueOf[QP3a.107]=2 | ValueOf[QP3a.107]=3] *INCLUDE DummyQP3b [107]
*IF [ValueOf[QP3a.108]=2 | ValueOf[QP3a.108]=3] *INCLUDE DummyQP3b [108]
*IF [ValueOf[QP3a.109]=2 | ValueOf[QP3a.109]=3] *INCLUDE DummyQP3b [109]
*IF [ValueOf[QP3a.110]=2 | ValueOf[QP3a.110]=3] *INCLUDE DummyQP3b [110]
*IF [ValueOf[QP3a.111]=2 | ValueOf[QP3a.111]=3] *INCLUDE DummyQP3b [111]
*IF [ValueOf[QP3a.112]=2 | ValueOf[QP3a.112]=3] *INCLUDE DummyQP3b [112]
*IF [ValueOf[QP3a.113]=2 | ValueOf[QP3a.113]=3] *INCLUDE DummyQP3b [113]
*IF [ValueOf[QP3a.114]=2 | ValueOf[QP3a.114]=3] *INCLUDE DummyQP3b [114]
*IF [ValueOf[QP3a.115]=2 | ValueOf[QP3a.115]=3] *INCLUDE DummyQP3b [115]
*IF [ValueOf[QP3a.116]=2 | ValueOf[QP3a.116]=3] *INCLUDE DummyQP3b [116]
*IF [ValueOf[QP3a.117]=2 | ValueOf[QP3a.117]=3] *INCLUDE DummyQP3b [117]
*IF [ValueOf[QP3a.118]=2 | ValueOf[QP3a.118]=3] *INCLUDE DummyQP3b [118]
*IF [ValueOf[QP3a.119]=2 | ValueOf[QP3a.119]=3] *INCLUDE DummyQP3b [119]
*IF [ValueOf[QP3a.120]=2 | ValueOf[QP3a.120]=3] *INCLUDE DummyQP3b [120]
*IF [ValueOf[QP3a.121]=2 | ValueOf[QP3a.121]=3] *INCLUDE DummyQP3b [121]
*IF [ValueOf[QP3a.122]=2 | ValueOf[QP3a.122]=3] *INCLUDE DummyQP3b [122]
*IF [ValueOf[QP3a.123]=2 | ValueOf[QP3a.123]=3] *INCLUDE DummyQP3b [123]
*IF [ValueOf[QP3a.124]=2 | ValueOf[QP3a.124]=3] *INCLUDE DummyQP3b [124]
*IF [ValueOf[QP3a.125]=2 | ValueOf[QP3a.125]=3] *INCLUDE DummyQP3b [125]
*IF [ValueOf[QP3a.126]=2 | ValueOf[QP3a.126]=3] *INCLUDE DummyQP3b [126]
*IF [ValueOf[QP3a.127]=2 | ValueOf[QP3a.127]=3] *INCLUDE DummyQP3b [127]
*IF [ValueOf[QP3a.128]=2 | ValueOf[QP3a.128]=3] *INCLUDE DummyQP3b [128]
*IF [ValueOf[QP3a.129]=2 | ValueOf[QP3a.129]=3] *INCLUDE DummyQP3b [129]
*IF [ValueOf[QP3a.130]=2 | ValueOf[QP3a.130]=3] *INCLUDE DummyQP3b [130]
*IF [ValueOf[QP3a.131]=2 | ValueOf[QP3a.131]=3] *INCLUDE DummyQP3b [131]
*IF [ValueOf[QP3a.132]=2 | ValueOf[QP3a.132]=3] *INCLUDE DummyQP3b [132]
*IF [ValueOf[QP3a.133]=2 | ValueOf[QP3a.133]=3] *INCLUDE DummyQP3b [133]
*IF [ValueOf[QP3a.134]=2 | ValueOf[QP3a.134]=3] *INCLUDE DummyQP3b [134]
*IF [ValueOf[QP3a.135]=2 | ValueOf[QP3a.135]=3] *INCLUDE DummyQP3b [135]
*IF [ValueOf[QP3a.136]=2 | ValueOf[QP3a.136]=3] *INCLUDE DummyQP3b [136]
*IF [ValueOf[QP3a.137]=2 | ValueOf[QP3a.137]=3] *INCLUDE DummyQP3b [137]
*IF [ValueOf[QP3a.138]=2 | ValueOf[QP3a.138]=3] *INCLUDE DummyQP3b [138]
*IF [ValueOf[QP3a.139]=2 | ValueOf[QP3a.139]=3] *INCLUDE DummyQP3b [139]
*IF [ValueOf[QP3a.140]=2 | ValueOf[QP3a.140]=3] *INCLUDE DummyQP3b [140]
*IF [ValueOf[QP3a.141]=2 | ValueOf[QP3a.141]=3] *INCLUDE DummyQP3b [141]
*IF [ValueOf[QP3a.142]=2 | ValueOf[QP3a.142]=3] *INCLUDE DummyQP3b [142]
*IF [ValueOf[QP3a.143]=2 | ValueOf[QP3a.143]=3] *INCLUDE DummyQP3b [143]
*IF [ValueOf[QP3a.144]=2 | ValueOf[QP3a.144]=3] *INCLUDE DummyQP3b [144]
*IF [ValueOf[QP3a.145]=2 | ValueOf[QP3a.145]=3] *INCLUDE DummyQP3b [145]
*IF [ValueOf[QP3a.146]=2 | ValueOf[QP3a.146]=3] *INCLUDE DummyQP3b [146]
*IF [ValueOf[QP3a.147]=2 | ValueOf[QP3a.147]=3] *INCLUDE DummyQP3b [147]
*IF [ValueOf[QP3a.148]=2 | ValueOf[QP3a.148]=3] *INCLUDE DummyQP3b [148]
*IF [ValueOf[QP3a.149]=2 | ValueOf[QP3a.149]=3] *INCLUDE DummyQP3b [149]
*IF [ValueOf[QP3a.150]=2 | ValueOf[QP3a.150]=3] *INCLUDE DummyQP3b [150]
*IF [ValueOf[QP3a.151]=2 | ValueOf[QP3a.151]=3] *INCLUDE DummyQP3b [151]
*IF [ValueOf[QP3a.152]=2 | ValueOf[QP3a.152]=3] *INCLUDE DummyQP3b [152]
*IF [ValueOf[QP3a.153]=2 | ValueOf[QP3a.153]=3] *INCLUDE DummyQP3b [153]
*IF [ValueOf[QP3a.154]=2 | ValueOf[QP3a.154]=3] *INCLUDE DummyQP3b [154]
*IF [ValueOf[QP3a.155]=2 | ValueOf[QP3a.155]=3] *INCLUDE DummyQP3b [155]
*IF [ValueOf[QP3a.156]=2 | ValueOf[QP3a.156]=3] *INCLUDE DummyQP3b [156]
*IF [ValueOf[QP3a.157]=2 | ValueOf[QP3a.157]=3] *INCLUDE DummyQP3b [157]
*IF [ValueOf[QP3a.158]=2 | ValueOf[QP3a.158]=3] *INCLUDE DummyQP3b [158]
*IF [ValueOf[QP3a.159]=2 | ValueOf[QP3a.159]=3] *INCLUDE DummyQP3b [159]
*IF [ValueOf[QP3a.160]=2 | ValueOf[QP3a.160]=3] *INCLUDE DummyQP3b [160]
*IF [ValueOf[QP3a.161]=2 | ValueOf[QP3a.161]=3] *INCLUDE DummyQP3b [161]
*IF [ValueOf[QP3a.162]=2 | ValueOf[QP3a.162]=3] *INCLUDE DummyQP3b [162]
*IF [ValueOf[QP3a.163]=2 | ValueOf[QP3a.163]=3] *INCLUDE DummyQP3b [163]
*IF [ValueOf[QP3a.164]=2 | ValueOf[QP3a.164]=3] *INCLUDE DummyQP3b [164]
*IF [ValueOf[QP3a.165]=2 | ValueOf[QP3a.165]=3] *INCLUDE DummyQP3b [165]
*IF [ValueOf[QP3a.398]=2 | ValueOf[QP3a.398]=3] *INCLUDE DummyQP3b [398]
*IF [ValueOf[QP3a.166]=2 | ValueOf[QP3a.166]=3] *INCLUDE DummyQP3b [166]
*IF [ValueOf[QP3a.167]=2 | ValueOf[QP3a.167]=3] *INCLUDE DummyQP3b [167]
*IF [ValueOf[QP3a.168]=2 | ValueOf[QP3a.168]=3] *INCLUDE DummyQP3b [168]
*IF [ValueOf[QP3a.169]=2 | ValueOf[QP3a.169]=3] *INCLUDE DummyQP3b [169]
*IF [ValueOf[QP3a.170]=2 | ValueOf[QP3a.170]=3] *INCLUDE DummyQP3b [170]
*IF [ValueOf[QP3a.171]=2 | ValueOf[QP3a.171]=3] *INCLUDE DummyQP3b [171]
*IF [ValueOf[QP3a.172]=2 | ValueOf[QP3a.172]=3] *INCLUDE DummyQP3b [172]
*IF [ValueOf[QP3a.173]=2 | ValueOf[QP3a.173]=3] *INCLUDE DummyQP3b [173]
*IF [ValueOf[QP3a.174]=2 | ValueOf[QP3a.174]=3] *INCLUDE DummyQP3b [174]
*IF [ValueOf[QP3a.175]=2 | ValueOf[QP3a.175]=3] *INCLUDE DummyQP3b [175]
*IF [ValueOf[QP3a.176]=2 | ValueOf[QP3a.176]=3] *INCLUDE DummyQP3b [176]
*IF [ValueOf[QP3a.177]=2 | ValueOf[QP3a.177]=3] *INCLUDE DummyQP3b [177]
*IF [ValueOf[QP3a.178]=2 | ValueOf[QP3a.178]=3] *INCLUDE DummyQP3b [178]
*IF [ValueOf[QP3a.179]=2 | ValueOf[QP3a.179]=3] *INCLUDE DummyQP3b [179]
*IF [ValueOf[QP3a.180]=2 | ValueOf[QP3a.180]=3] *INCLUDE DummyQP3b [180]
*IF [ValueOf[QP3a.181]=2 | ValueOf[QP3a.181]=3] *INCLUDE DummyQP3b [181]
*IF [ValueOf[QP3a.182]=2 | ValueOf[QP3a.182]=3] *INCLUDE DummyQP3b [182]
*IF [ValueOf[QP3a.183]=2 | ValueOf[QP3a.183]=3] *INCLUDE DummyQP3b [183]
*IF [ValueOf[QP3a.184]=2 | ValueOf[QP3a.184]=3] *INCLUDE DummyQP3b [184]
*IF [ValueOf[QP3a.185]=2 | ValueOf[QP3a.185]=3] *INCLUDE DummyQP3b [185]
*IF [ValueOf[QP3a.498]=2 | ValueOf[QP3a.498]=3] *INCLUDE DummyQP3b [498]
*IF [ValueOf[QP3a.186]=2 | ValueOf[QP3a.186]=3] *INCLUDE DummyQP3b [186]
*IF [ValueOf[QP3a.187]=2 | ValueOf[QP3a.187]=3] *INCLUDE DummyQP3b [187]
*IF [ValueOf[QP3a.188]=2 | ValueOf[QP3a.188]=3] *INCLUDE DummyQP3b [188]
*IF [ValueOf[QP3a.189]=2 | ValueOf[QP3a.189]=3] *INCLUDE DummyQP3b [189]
*IF [ValueOf[QP3a.190]=2 | ValueOf[QP3a.190]=3] *INCLUDE DummyQP3b [190]
*IF [ValueOf[QP3a.191]=2 | ValueOf[QP3a.191]=3] *INCLUDE DummyQP3b [191]
*IF [ValueOf[QP3a.192]=2 | ValueOf[QP3a.192]=3] *INCLUDE DummyQP3b [192]
*IF [ValueOf[QP3a.193]=2 | ValueOf[QP3a.193]=3] *INCLUDE DummyQP3b [193]
*IF [ValueOf[QP3a.194]=2 | ValueOf[QP3a.194]=3] *INCLUDE DummyQP3b [194]
*IF [ValueOf[QP3a.195]=2 | ValueOf[QP3a.195]=3] *INCLUDE DummyQP3b [195]
*IF [ValueOf[QP3a.196]=2 | ValueOf[QP3a.196]=3] *INCLUDE DummyQP3b [196]
*IF [ValueOf[QP3a.197]=2 | ValueOf[QP3a.197]=3] *INCLUDE DummyQP3b [197]
*IF [ValueOf[QP3a.198]=2 | ValueOf[QP3a.198]=3] *INCLUDE DummyQP3b [198]
*IF [ValueOf[QP3a.199]=2 | ValueOf[QP3a.199]=3] *INCLUDE DummyQP3b [199]
*IF [ValueOf[QP3a.200]=2 | ValueOf[QP3a.200]=3] *INCLUDE DummyQP3b [200]
*IF [ValueOf[QP3a.201]=2 | ValueOf[QP3a.201]=3] *INCLUDE DummyQP3b [201]
*IF [ValueOf[QP3a.202]=2 | ValueOf[QP3a.202]=3] *INCLUDE DummyQP3b [202]
*IF [ValueOf[QP3a.203]=2 | ValueOf[QP3a.203]=3] *INCLUDE DummyQP3b [203]
*IF [ValueOf[QP3a.204]=2 | ValueOf[QP3a.204]=3] *INCLUDE DummyQP3b [204]
*IF [ValueOf[QP3a.205]=2 | ValueOf[QP3a.205]=3] *INCLUDE DummyQP3b [205]
*IF [ValueOf[QP3a.206]=2 | ValueOf[QP3a.206]=3] *INCLUDE DummyQP3b [206]
*IF [ValueOf[QP3a.207]=2 | ValueOf[QP3a.207]=3] *INCLUDE DummyQP3b [207]
*IF [ValueOf[QP3a.208]=2 | ValueOf[QP3a.208]=3] *INCLUDE DummyQP3b [208]
*IF [ValueOf[QP3a.209]=2 | ValueOf[QP3a.209]=3] *INCLUDE DummyQP3b [209]
*IF [ValueOf[QP3a.210]=2 | ValueOf[QP3a.210]=3] *INCLUDE DummyQP3b [210]
*IF [ValueOf[QP3a.211]=2 | ValueOf[QP3a.211]=3] *INCLUDE DummyQP3b [211]
*IF [ValueOf[QP3a.598]=2 | ValueOf[QP3a.598]=3] *INCLUDE DummyQP3b [598]
*IF [ValueOf[QP3a.212]=2 | ValueOf[QP3a.212]=3] *INCLUDE DummyQP3b [212]
*IF [ValueOf[QP3a.213]=2 | ValueOf[QP3a.213]=3] *INCLUDE DummyQP3b [213]
*IF [ValueOf[QP3a.214]=2 | ValueOf[QP3a.214]=3] *INCLUDE DummyQP3b [214]
*IF [ValueOf[QP3a.698]=2 | ValueOf[QP3a.698]=3] *INCLUDE DummyQP3b [698]
*IF [ValueOf[QP3a.215]=2 | ValueOf[QP3a.215]=3] *INCLUDE DummyQP3b [215]
*IF [ValueOf[QP3a.216]=2 | ValueOf[QP3a.216]=3] *INCLUDE DummyQP3b [216]
*IF [ValueOf[QP3a.217]=2 | ValueOf[QP3a.217]=3] *INCLUDE DummyQP3b [217]
*IF [ValueOf[QP3a.218]=2 | ValueOf[QP3a.218]=3] *INCLUDE DummyQP3b [218]
*IF [ValueOf[QP3a.219]=2 | ValueOf[QP3a.219]=3] *INCLUDE DummyQP3b [219]
*IF [ValueOf[QP3a.220]=2 | ValueOf[QP3a.220]=3] *INCLUDE DummyQP3b [220]
*IF [ValueOf[QP3a.798]=2 | ValueOf[QP3a.798]=3] *INCLUDE DummyQP3b [798]
*IF [ValueOf[QP3a.221]=2 | ValueOf[QP3a.221]=3] *INCLUDE DummyQP3b [221]
*IF [ValueOf[QP3a.222]=2 | ValueOf[QP3a.222]=3] *INCLUDE DummyQP3b [222]
*IF [ValueOf[QP3a.223]=2 | ValueOf[QP3a.223]=3] *INCLUDE DummyQP3b [223]
*IF [ValueOf[QP3a.224]=2 | ValueOf[QP3a.224]=3] *INCLUDE DummyQP3b [224]
*IF [ValueOf[QP3a.225]=2 | ValueOf[QP3a.225]=3] *INCLUDE DummyQP3b [225]
*IF [ValueOf[QP3a.226]=2 | ValueOf[QP3a.226]=3] *INCLUDE DummyQP3b [226]
*IF [ValueOf[QP3a.227]=2 | ValueOf[QP3a.227]=3] *INCLUDE DummyQP3b [227]
*IF [ValueOf[QP3a.228]=2 | ValueOf[QP3a.228]=3] *INCLUDE DummyQP3b [228]
*IF [ValueOf[QP3a.898]=2 | ValueOf[QP3a.898]=3] *INCLUDE DummyQP3b [898]
*IF [ValueOf[QP3a.301]=2 | ValueOf[QP3a.301]=3] *INCLUDE DummyQP3b [301]
*IF [ValueOf[QP3a.302]=2 | ValueOf[QP3a.302]=3] *INCLUDE DummyQP3b [302]
*IF [ValueOf[QP3a.303]=2 | ValueOf[QP3a.303]=3] *INCLUDE DummyQP3b [303]
*IF [ValueOf[QP3a.311]=2 | ValueOf[QP3a.311]=3] *INCLUDE DummyQP3b [311]
*IF [ValueOf[QP3a.312]=2 | ValueOf[QP3a.312]=3] *INCLUDE DummyQP3b [312]
*IF [ValueOf[QP3a.313]=2 | ValueOf[QP3a.313]=3] *INCLUDE DummyQP3b [313]
*IF [ValueOf[QP3a.321]=2 | ValueOf[QP3a.321]=3] *INCLUDE DummyQP3b [321]
*IF [ValueOf[QP3a.322]=2 | ValueOf[QP3a.322]=3] *INCLUDE DummyQP3b [322]
*IF [ValueOf[QP3a.323]=2 | ValueOf[QP3a.323]=3] *INCLUDE DummyQP3b [323]
*IF [ValueOf[QP3a.331]=2 | ValueOf[QP3a.331]=3] *INCLUDE DummyQP3b [331]
*IF [ValueOf[QP3a.332]=2 | ValueOf[QP3a.332]=3] *INCLUDE DummyQP3b [332]
*IF [ValueOf[QP3a.333]=2 | ValueOf[QP3a.333]=3] *INCLUDE DummyQP3b [333]
*IF [ValueOf[QP3a.341]=2 | ValueOf[QP3a.341]=3] *INCLUDE DummyQP3b [341]
*IF [ValueOf[QP3a.342]=2 | ValueOf[QP3a.342]=3] *INCLUDE DummyQP3b [342]
*IF [ValueOf[QP3a.343]=2 | ValueOf[QP3a.343]=3] *INCLUDE DummyQP3b [343]
*IF [ValueOf[QP3a.351]=2 | ValueOf[QP3a.351]=3] *INCLUDE DummyQP3b [351]
*IF [ValueOf[QP3a.352]=2 | ValueOf[QP3a.352]=3] *INCLUDE DummyQP3b [352]
*IF [ValueOf[QP3a.353]=2 | ValueOf[QP3a.353]=3] *INCLUDE DummyQP3b [353]
*IF [ValueOf[QP3a.361]=2 | ValueOf[QP3a.361]=3] *INCLUDE DummyQP3b [361]
*IF [ValueOf[QP3a.362]=2 | ValueOf[QP3a.362]=3] *INCLUDE DummyQP3b [362]
*IF [ValueOf[QP3a.363]=2 | ValueOf[QP3a.363]=3] *INCLUDE DummyQP3b [363]
*IF [ValueOf[QP3a.371]=2 | ValueOf[QP3a.371]=3] *INCLUDE DummyQP3b [371]
*IF [ValueOf[QP3a.372]=2 | ValueOf[QP3a.372]=3] *INCLUDE DummyQP3b [372]
*IF [ValueOf[QP3a.373]=2 | ValueOf[QP3a.373]=3] *INCLUDE DummyQP3b [373]
*IF [ValueOf[QP3a.381]=2 | ValueOf[QP3a.381]=3] *INCLUDE DummyQP3b [381]
*IF [ValueOf[QP3a.382]=2 | ValueOf[QP3a.382]=3] *INCLUDE DummyQP3b [382]
*IF [ValueOf[QP3a.383]=2 | ValueOf[QP3a.383]=3] *INCLUDE DummyQP3b [383]
*IF [ValueOf[QP3a.391]=2 | ValueOf[QP3a.391]=3] *INCLUDE DummyQP3b [391]
*IF [ValueOf[QP3a.392]=2 | ValueOf[QP3a.392]=3] *INCLUDE DummyQP3b [392]
*IF [ValueOf[QP3a.393]=2 | ValueOf[QP3a.393]=3] *INCLUDE DummyQP3b [393]
*IF [ValueOf[QP3a.401]=2 | ValueOf[QP3a.401]=3] *INCLUDE DummyQP3b [401]
*IF [ValueOf[QP3a.402]=2 | ValueOf[QP3a.402]=3] *INCLUDE DummyQP3b [402]
*IF [ValueOf[QP3a.403]=2 | ValueOf[QP3a.403]=3] *INCLUDE DummyQP3b [403]
*IF [ValueOf[QP3a.411]=2 | ValueOf[QP3a.411]=3] *INCLUDE DummyQP3b [411]
*IF [ValueOf[QP3a.412]=2 | ValueOf[QP3a.412]=3] *INCLUDE DummyQP3b [412]
*IF [ValueOf[QP3a.413]=2 | ValueOf[QP3a.413]=3] *INCLUDE DummyQP3b [413]
*IF [ValueOf[QP3a.421]=2 | ValueOf[QP3a.421]=3] *INCLUDE DummyQP3b [421]
*IF [ValueOf[QP3a.422]=2 | ValueOf[QP3a.422]=3] *INCLUDE DummyQP3b [422]
*IF [ValueOf[QP3a.423]=2 | ValueOf[QP3a.423]=3] *INCLUDE DummyQP3b [423]
*IF [ValueOf[QP3a.431]=2 | ValueOf[QP3a.431]=3] *INCLUDE DummyQP3b [431]
*IF [ValueOf[QP3a.432]=2 | ValueOf[QP3a.432]=3] *INCLUDE DummyQP3b [432]
*IF [ValueOf[QP3a.433]=2 | ValueOf[QP3a.433]=3] *INCLUDE DummyQP3b [433]
 
 
 

*GRIDLIST "GridQP3b"
1:Less than Rs. 500
2:Rs. 501 – Rs. 1000
3:Rs. 1001 – Rs. 2000
4:Rs. 2001 – Rs. 5000
5:Rs. 5001 – Rs. 10000
6:Above Rs. 10000
7:Don't know

*QUESTION QP3b *GRIDSR *USEGRIDLIST "GridQP3b" *INCLUDE [DummyQP3b] *IF [NumberOfResponse[DummyQP3b]>0] *IMGADJBY 400
QP3b. Can you tell what is the annual fee of {DummyQP3b} ? SA for Each Card
*USELIST "CCListImage"


*QUESTION QP4exp *MR *INCLUDE [CardBrand]
QP4. Can you tell what card have you spent in the last month? Please consider only the payments made through credit cards in last month. 
*USELIST "CCListImage"
 
*QUESTION QP4 *NUMLISTTOTAL *INCLUDE [QP4exp] *IF [NumberOfResponse[QP4exp]>0] *IMGADJBY 400
QP4. Can you tell what is the total amount spent in the last month through each these cards? Please include all the payments made through the credit cards.
*USELIST "CCListImage"
999:Total *NMUL *NOCON *MANDATORY

*QUESTION QP4999Msg *INFO *IF [ValueOf[QP4.999]>100000]
In Total, your last month expense through Credit Card was <font color="#FF73FF">{QP4.999}</b>. It seems to be very HIGH. Can you confirm the Last Month Bill value again on this card

*GRIDLIST "GridQP5"
1:Co Branded
2:Not Co branded

*QUESTION QP5 *GRIDSR *USEGRIDLIST "GridQP5" *INCLUDE [CardBrand] *IMGADJBY 400
QP5. Which of these are co-branded credit cards?  Co-branded cards where a credit card is given in partnership with another brand? Such as Kotak Swiggy Credit card , ICICI Amazon Pay card etc.  in SA for Each Card
*USELIST "CCListImage"

#*IF [QHD12d=1 | QHD12d=2]

*QUESTION QP6 *SR *IF [QHD12d=1 | QHD12d=2]
QP6. Do any of your credit cards have tap & pay option? SA
1:Yes, I have Contactless payment (Tap and Pay) credit cards
2:No, all my credit cards are swipe cards


*GRIDLIST "GridQP7"
1:Daily /Almost Daily
2:4-5 times a week
3:1-2 times a week
4:Weekly
5:1-2 times a month
6:Once a month
7:Occasionally
8:Rarely

#*SHOWASFORM

*QUESTION QP7 *GRIDSR *USEGRIDLIST "GridQP7" *INCLUDE [CardBrand] *IMGADJBY 400 
QP7. How frequently do you use each of these cards. [SA]
*USELIST "CCListImage"


#*QUESTION DummyQP8 *MR *DUMMY2
#QP8. Can you tell which of these cards are free & for which you pay an annual fee? SA for Each Card
#*USELIST "CCList"
#
#
#*IF [ValueOf[QP7.1]=2 | ValueOf[QP7.1]=3] *INCLUDE DummyQP8 [1]
#*IF [ValueOf[QP7.2]=2 | ValueOf[QP7.2]=3] *INCLUDE DummyQP8 [2]
#*IF [ValueOf[QP7.3]=2 | ValueOf[QP7.3]=3] *INCLUDE DummyQP8 [3]
#*IF [ValueOf[QP7.4]=2 | ValueOf[QP7.4]=3] *INCLUDE DummyQP8 [4]
#*IF [ValueOf[QP7.5]=2 | ValueOf[QP7.5]=3] *INCLUDE DummyQP8 [5]
#*IF [ValueOf[QP7.6]=2 | ValueOf[QP7.6]=3] *INCLUDE DummyQP8 [6]
#*IF [ValueOf[QP7.7]=2 | ValueOf[QP7.7]=3] *INCLUDE DummyQP8 [7]
#*IF [ValueOf[QP7.8]=2 | ValueOf[QP7.8]=3] *INCLUDE DummyQP8 [8]
#*IF [ValueOf[QP7.9]=2 | ValueOf[QP7.9]=3] *INCLUDE DummyQP8 [9]
#*IF [ValueOf[QP7.10]=2 | ValueOf[QP7.10]=3] *INCLUDE DummyQP8 [10]
#*IF [ValueOf[QP7.11]=2 | ValueOf[QP7.11]=3] *INCLUDE DummyQP8 [11]
#*IF [ValueOf[QP7.12]=2 | ValueOf[QP7.12]=3] *INCLUDE DummyQP8 [12]
#*IF [ValueOf[QP7.13]=2 | ValueOf[QP7.13]=3] *INCLUDE DummyQP8 [13]
#*IF [ValueOf[QP7.14]=2 | ValueOf[QP7.14]=3] *INCLUDE DummyQP8 [14]
#*IF [ValueOf[QP7.15]=2 | ValueOf[QP7.15]=3] *INCLUDE DummyQP8 [15]
#*IF [ValueOf[QP7.16]=2 | ValueOf[QP7.16]=3] *INCLUDE DummyQP8 [16]
#*IF [ValueOf[QP7.17]=2 | ValueOf[QP7.17]=3] *INCLUDE DummyQP8 [17]
#*IF [ValueOf[QP7.18]=2 | ValueOf[QP7.18]=3] *INCLUDE DummyQP8 [18]
#*IF [ValueOf[QP7.19]=2 | ValueOf[QP7.19]=3] *INCLUDE DummyQP8 [19]
#*IF [ValueOf[QP7.20]=2 | ValueOf[QP7.20]=3] *INCLUDE DummyQP8 [20]
#*IF [ValueOf[QP7.21]=2 | ValueOf[QP7.21]=3] *INCLUDE DummyQP8 [21]
#*IF [ValueOf[QP7.22]=2 | ValueOf[QP7.22]=3] *INCLUDE DummyQP8 [22]
#*IF [ValueOf[QP7.23]=2 | ValueOf[QP7.23]=3] *INCLUDE DummyQP8 [23]
#*IF [ValueOf[QP7.24]=2 | ValueOf[QP7.24]=3] *INCLUDE DummyQP8 [24]
#*IF [ValueOf[QP7.25]=2 | ValueOf[QP7.25]=3] *INCLUDE DummyQP8 [25]
#*IF [ValueOf[QP7.26]=2 | ValueOf[QP7.26]=3] *INCLUDE DummyQP8 [26]
#*IF [ValueOf[QP7.27]=2 | ValueOf[QP7.27]=3] *INCLUDE DummyQP8 [27]
#*IF [ValueOf[QP7.28]=2 | ValueOf[QP7.28]=3] *INCLUDE DummyQP8 [28]
#*IF [ValueOf[QP7.29]=2 | ValueOf[QP7.29]=3] *INCLUDE DummyQP8 [29]
#*IF [ValueOf[QP7.30]=2 | ValueOf[QP7.30]=3] *INCLUDE DummyQP8 [30]
#*IF [ValueOf[QP7.31]=2 | ValueOf[QP7.31]=3] *INCLUDE DummyQP8 [31]
#*IF [ValueOf[QP7.32]=2 | ValueOf[QP7.32]=3] *INCLUDE DummyQP8 [32]
#*IF [ValueOf[QP7.33]=2 | ValueOf[QP7.33]=3] *INCLUDE DummyQP8 [33]
#*IF [ValueOf[QP7.34]=2 | ValueOf[QP7.34]=3] *INCLUDE DummyQP8 [34]
#*IF [ValueOf[QP7.35]=2 | ValueOf[QP7.35]=3] *INCLUDE DummyQP8 [35]
#*IF [ValueOf[QP7.36]=2 | ValueOf[QP7.36]=3] *INCLUDE DummyQP8 [36]
#*IF [ValueOf[QP7.1198]=2 | ValueOf[QP7.1198]=3] *INCLUDE DummyQP8 [1198]
#*IF [ValueOf[QP7.37]=2 | ValueOf[QP7.37]=3] *INCLUDE DummyQP8 [37]
#*IF [ValueOf[QP7.38]=2 | ValueOf[QP7.38]=3] *INCLUDE DummyQP8 [38]
#*IF [ValueOf[QP7.39]=2 | ValueOf[QP7.39]=3] *INCLUDE DummyQP8 [39]
#*IF [ValueOf[QP7.40]=2 | ValueOf[QP7.40]=3] *INCLUDE DummyQP8 [40]
#*IF [ValueOf[QP7.41]=2 | ValueOf[QP7.41]=3] *INCLUDE DummyQP8 [41]
#*IF [ValueOf[QP7.42]=2 | ValueOf[QP7.42]=3] *INCLUDE DummyQP8 [42]
#*IF [ValueOf[QP7.43]=2 | ValueOf[QP7.43]=3] *INCLUDE DummyQP8 [43]
#*IF [ValueOf[QP7.44]=2 | ValueOf[QP7.44]=3] *INCLUDE DummyQP8 [44]
#*IF [ValueOf[QP7.45]=2 | ValueOf[QP7.45]=3] *INCLUDE DummyQP8 [45]
#*IF [ValueOf[QP7.46]=2 | ValueOf[QP7.46]=3] *INCLUDE DummyQP8 [46]
#*IF [ValueOf[QP7.47]=2 | ValueOf[QP7.47]=3] *INCLUDE DummyQP8 [47]
#*IF [ValueOf[QP7.48]=2 | ValueOf[QP7.48]=3] *INCLUDE DummyQP8 [48]
#*IF [ValueOf[QP7.49]=2 | ValueOf[QP7.49]=3] *INCLUDE DummyQP8 [49]
#*IF [ValueOf[QP7.50]=2 | ValueOf[QP7.50]=3] *INCLUDE DummyQP8 [50]
#*IF [ValueOf[QP7.51]=2 | ValueOf[QP7.51]=3] *INCLUDE DummyQP8 [51]
#*IF [ValueOf[QP7.52]=2 | ValueOf[QP7.52]=3] *INCLUDE DummyQP8 [52]
#*IF [ValueOf[QP7.53]=2 | ValueOf[QP7.53]=3] *INCLUDE DummyQP8 [53]
#*IF [ValueOf[QP7.54]=2 | ValueOf[QP7.54]=3] *INCLUDE DummyQP8 [54]
#*IF [ValueOf[QP7.55]=2 | ValueOf[QP7.55]=3] *INCLUDE DummyQP8 [55]
#*IF [ValueOf[QP7.56]=2 | ValueOf[QP7.56]=3] *INCLUDE DummyQP8 [56]
#*IF [ValueOf[QP7.57]=2 | ValueOf[QP7.57]=3] *INCLUDE DummyQP8 [57]
#*IF [ValueOf[QP7.58]=2 | ValueOf[QP7.58]=3] *INCLUDE DummyQP8 [58]
#*IF [ValueOf[QP7.59]=2 | ValueOf[QP7.59]=3] *INCLUDE DummyQP8 [59]
#*IF [ValueOf[QP7.60]=2 | ValueOf[QP7.60]=3] *INCLUDE DummyQP8 [60]
#*IF [ValueOf[QP7.61]=2 | ValueOf[QP7.61]=3] *INCLUDE DummyQP8 [61]
#*IF [ValueOf[QP7.62]=2 | ValueOf[QP7.62]=3] *INCLUDE DummyQP8 [62]
#*IF [ValueOf[QP7.63]=2 | ValueOf[QP7.63]=3] *INCLUDE DummyQP8 [63]
#*IF [ValueOf[QP7.64]=2 | ValueOf[QP7.64]=3] *INCLUDE DummyQP8 [64]
#*IF [ValueOf[QP7.65]=2 | ValueOf[QP7.65]=3] *INCLUDE DummyQP8 [65]
#*IF [ValueOf[QP7.66]=2 | ValueOf[QP7.66]=3] *INCLUDE DummyQP8 [66]
#*IF [ValueOf[QP7.67]=2 | ValueOf[QP7.67]=3] *INCLUDE DummyQP8 [67]
#*IF [ValueOf[QP7.68]=2 | ValueOf[QP7.68]=3] *INCLUDE DummyQP8 [68]
#*IF [ValueOf[QP7.69]=2 | ValueOf[QP7.69]=3] *INCLUDE DummyQP8 [69]
#*IF [ValueOf[QP7.70]=2 | ValueOf[QP7.70]=3] *INCLUDE DummyQP8 [70]
#*IF [ValueOf[QP7.71]=2 | ValueOf[QP7.71]=3] *INCLUDE DummyQP8 [71]
#*IF [ValueOf[QP7.72]=2 | ValueOf[QP7.72]=3] *INCLUDE DummyQP8 [72]
#*IF [ValueOf[QP7.73]=2 | ValueOf[QP7.73]=3] *INCLUDE DummyQP8 [73]
#*IF [ValueOf[QP7.74]=2 | ValueOf[QP7.74]=3] *INCLUDE DummyQP8 [74]
#*IF [ValueOf[QP7.75]=2 | ValueOf[QP7.75]=3] *INCLUDE DummyQP8 [75]
#*IF [ValueOf[QP7.76]=2 | ValueOf[QP7.76]=3] *INCLUDE DummyQP8 [76]
#*IF [ValueOf[QP7.77]=2 | ValueOf[QP7.77]=3] *INCLUDE DummyQP8 [77]
#*IF [ValueOf[QP7.29]=2 | ValueOf[QP7.29]=3] *INCLUDE DummyQP8 [29]
#*IF [ValueOf[QP7.78]=2 | ValueOf[QP7.78]=3] *INCLUDE DummyQP8 [78]
#*IF [ValueOf[QP7.79]=2 | ValueOf[QP7.79]=3] *INCLUDE DummyQP8 [79]
#*IF [ValueOf[QP7.80]=2 | ValueOf[QP7.80]=3] *INCLUDE DummyQP8 [80]
#*IF [ValueOf[QP7.81]=2 | ValueOf[QP7.81]=3] *INCLUDE DummyQP8 [81]
#*IF [ValueOf[QP7.82]=2 | ValueOf[QP7.82]=3] *INCLUDE DummyQP8 [82]
#*IF [ValueOf[QP7.83]=2 | ValueOf[QP7.83]=3] *INCLUDE DummyQP8 [83]
#*IF [ValueOf[QP7.84]=2 | ValueOf[QP7.84]=3] *INCLUDE DummyQP8 [84]
#*IF [ValueOf[QP7.85]=2 | ValueOf[QP7.85]=3] *INCLUDE DummyQP8 [85]
#*IF [ValueOf[QP7.86]=2 | ValueOf[QP7.86]=3] *INCLUDE DummyQP8 [86]
#*IF [ValueOf[QP7.87]=2 | ValueOf[QP7.87]=3] *INCLUDE DummyQP8 [87]
#*IF [ValueOf[QP7.88]=2 | ValueOf[QP7.88]=3] *INCLUDE DummyQP8 [88]
#*IF [ValueOf[QP7.89]=2 | ValueOf[QP7.89]=3] *INCLUDE DummyQP8 [89]
#*IF [ValueOf[QP7.90]=2 | ValueOf[QP7.90]=3] *INCLUDE DummyQP8 [90]
#*IF [ValueOf[QP7.91]=2 | ValueOf[QP7.91]=3] *INCLUDE DummyQP8 [91]
#*IF [ValueOf[QP7.92]=2 | ValueOf[QP7.92]=3] *INCLUDE DummyQP8 [92]
#*IF [ValueOf[QP7.93]=2 | ValueOf[QP7.93]=3] *INCLUDE DummyQP8 [93]
#*IF [ValueOf[QP7.94]=2 | ValueOf[QP7.94]=3] *INCLUDE DummyQP8 [94]
#*IF [ValueOf[QP7.95]=2 | ValueOf[QP7.95]=3] *INCLUDE DummyQP8 [95]
#*IF [ValueOf[QP7.96]=2 | ValueOf[QP7.96]=3] *INCLUDE DummyQP8 [96]
#*IF [ValueOf[QP7.97]=2 | ValueOf[QP7.97]=3] *INCLUDE DummyQP8 [97]
#*IF [ValueOf[QP7.98]=2 | ValueOf[QP7.98]=3] *INCLUDE DummyQP8 [98]
#*IF [ValueOf[QP7.99]=2 | ValueOf[QP7.99]=3] *INCLUDE DummyQP8 [99]
#*IF [ValueOf[QP7.100]=2 | ValueOf[QP7.100]=3] *INCLUDE DummyQP8 [100]
#*IF [ValueOf[QP7.101]=2 | ValueOf[QP7.101]=3] *INCLUDE DummyQP8 [101]
#*IF [ValueOf[QP7.102]=2 | ValueOf[QP7.102]=3] *INCLUDE DummyQP8 [102]
#*IF [ValueOf[QP7.103]=2 | ValueOf[QP7.103]=3] *INCLUDE DummyQP8 [103]
#*IF [ValueOf[QP7.104]=2 | ValueOf[QP7.104]=3] *INCLUDE DummyQP8 [104]
#*IF [ValueOf[QP7.105]=2 | ValueOf[QP7.105]=3] *INCLUDE DummyQP8 [105]
#*IF [ValueOf[QP7.106]=2 | ValueOf[QP7.106]=3] *INCLUDE DummyQP8 [106]
#*IF [ValueOf[QP7.107]=2 | ValueOf[QP7.107]=3] *INCLUDE DummyQP8 [107]
#*IF [ValueOf[QP7.108]=2 | ValueOf[QP7.108]=3] *INCLUDE DummyQP8 [108]
#*IF [ValueOf[QP7.109]=2 | ValueOf[QP7.109]=3] *INCLUDE DummyQP8 [109]
#*IF [ValueOf[QP7.110]=2 | ValueOf[QP7.110]=3] *INCLUDE DummyQP8 [110]
#*IF [ValueOf[QP7.111]=2 | ValueOf[QP7.111]=3] *INCLUDE DummyQP8 [111]
#*IF [ValueOf[QP7.112]=2 | ValueOf[QP7.112]=3] *INCLUDE DummyQP8 [112]
#*IF [ValueOf[QP7.113]=2 | ValueOf[QP7.113]=3] *INCLUDE DummyQP8 [113]
#*IF [ValueOf[QP7.114]=2 | ValueOf[QP7.114]=3] *INCLUDE DummyQP8 [114]
#*IF [ValueOf[QP7.115]=2 | ValueOf[QP7.115]=3] *INCLUDE DummyQP8 [115]
#*IF [ValueOf[QP7.116]=2 | ValueOf[QP7.116]=3] *INCLUDE DummyQP8 [116]
#*IF [ValueOf[QP7.117]=2 | ValueOf[QP7.117]=3] *INCLUDE DummyQP8 [117]
#*IF [ValueOf[QP7.118]=2 | ValueOf[QP7.118]=3] *INCLUDE DummyQP8 [118]
#*IF [ValueOf[QP7.119]=2 | ValueOf[QP7.119]=3] *INCLUDE DummyQP8 [119]
#*IF [ValueOf[QP7.120]=2 | ValueOf[QP7.120]=3] *INCLUDE DummyQP8 [120]
#*IF [ValueOf[QP7.121]=2 | ValueOf[QP7.121]=3] *INCLUDE DummyQP8 [121]
#*IF [ValueOf[QP7.122]=2 | ValueOf[QP7.122]=3] *INCLUDE DummyQP8 [122]
#*IF [ValueOf[QP7.123]=2 | ValueOf[QP7.123]=3] *INCLUDE DummyQP8 [123]
#*IF [ValueOf[QP7.124]=2 | ValueOf[QP7.124]=3] *INCLUDE DummyQP8 [124]
#*IF [ValueOf[QP7.125]=2 | ValueOf[QP7.125]=3] *INCLUDE DummyQP8 [125]
#*IF [ValueOf[QP7.126]=2 | ValueOf[QP7.126]=3] *INCLUDE DummyQP8 [126]
#*IF [ValueOf[QP7.127]=2 | ValueOf[QP7.127]=3] *INCLUDE DummyQP8 [127]
#*IF [ValueOf[QP7.128]=2 | ValueOf[QP7.128]=3] *INCLUDE DummyQP8 [128]
#*IF [ValueOf[QP7.129]=2 | ValueOf[QP7.129]=3] *INCLUDE DummyQP8 [129]
#*IF [ValueOf[QP7.130]=2 | ValueOf[QP7.130]=3] *INCLUDE DummyQP8 [130]
#*IF [ValueOf[QP7.131]=2 | ValueOf[QP7.131]=3] *INCLUDE DummyQP8 [131]
#*IF [ValueOf[QP7.132]=2 | ValueOf[QP7.132]=3] *INCLUDE DummyQP8 [132]
#*IF [ValueOf[QP7.133]=2 | ValueOf[QP7.133]=3] *INCLUDE DummyQP8 [133]
#*IF [ValueOf[QP7.134]=2 | ValueOf[QP7.134]=3] *INCLUDE DummyQP8 [134]
#*IF [ValueOf[QP7.135]=2 | ValueOf[QP7.135]=3] *INCLUDE DummyQP8 [135]
#*IF [ValueOf[QP7.136]=2 | ValueOf[QP7.136]=3] *INCLUDE DummyQP8 [136]
#*IF [ValueOf[QP7.137]=2 | ValueOf[QP7.137]=3] *INCLUDE DummyQP8 [137]
#*IF [ValueOf[QP7.138]=2 | ValueOf[QP7.138]=3] *INCLUDE DummyQP8 [138]
#*IF [ValueOf[QP7.139]=2 | ValueOf[QP7.139]=3] *INCLUDE DummyQP8 [139]
#*IF [ValueOf[QP7.140]=2 | ValueOf[QP7.140]=3] *INCLUDE DummyQP8 [140]
#*IF [ValueOf[QP7.141]=2 | ValueOf[QP7.141]=3] *INCLUDE DummyQP8 [141]
#*IF [ValueOf[QP7.142]=2 | ValueOf[QP7.142]=3] *INCLUDE DummyQP8 [142]
#*IF [ValueOf[QP7.143]=2 | ValueOf[QP7.143]=3] *INCLUDE DummyQP8 [143]
#*IF [ValueOf[QP7.144]=2 | ValueOf[QP7.144]=3] *INCLUDE DummyQP8 [144]
#*IF [ValueOf[QP7.145]=2 | ValueOf[QP7.145]=3] *INCLUDE DummyQP8 [145]
#*IF [ValueOf[QP7.146]=2 | ValueOf[QP7.146]=3] *INCLUDE DummyQP8 [146]
#*IF [ValueOf[QP7.147]=2 | ValueOf[QP7.147]=3] *INCLUDE DummyQP8 [147]
#*IF [ValueOf[QP7.148]=2 | ValueOf[QP7.148]=3] *INCLUDE DummyQP8 [148]
#*IF [ValueOf[QP7.149]=2 | ValueOf[QP7.149]=3] *INCLUDE DummyQP8 [149]
#*IF [ValueOf[QP7.150]=2 | ValueOf[QP7.150]=3] *INCLUDE DummyQP8 [150]
#*IF [ValueOf[QP7.151]=2 | ValueOf[QP7.151]=3] *INCLUDE DummyQP8 [151]
#*IF [ValueOf[QP7.152]=2 | ValueOf[QP7.152]=3] *INCLUDE DummyQP8 [152]
#*IF [ValueOf[QP7.153]=2 | ValueOf[QP7.153]=3] *INCLUDE DummyQP8 [153]
#*IF [ValueOf[QP7.154]=2 | ValueOf[QP7.154]=3] *INCLUDE DummyQP8 [154]
#*IF [ValueOf[QP7.155]=2 | ValueOf[QP7.155]=3] *INCLUDE DummyQP8 [155]
#*IF [ValueOf[QP7.156]=2 | ValueOf[QP7.156]=3] *INCLUDE DummyQP8 [156]
#*IF [ValueOf[QP7.157]=2 | ValueOf[QP7.157]=3] *INCLUDE DummyQP8 [157]
#*IF [ValueOf[QP7.158]=2 | ValueOf[QP7.158]=3] *INCLUDE DummyQP8 [158]
#*IF [ValueOf[QP7.159]=2 | ValueOf[QP7.159]=3] *INCLUDE DummyQP8 [159]
#*IF [ValueOf[QP7.160]=2 | ValueOf[QP7.160]=3] *INCLUDE DummyQP8 [160]
#*IF [ValueOf[QP7.161]=2 | ValueOf[QP7.161]=3] *INCLUDE DummyQP8 [161]
#*IF [ValueOf[QP7.162]=2 | ValueOf[QP7.162]=3] *INCLUDE DummyQP8 [162]
#*IF [ValueOf[QP7.163]=2 | ValueOf[QP7.163]=3] *INCLUDE DummyQP8 [163]
#*IF [ValueOf[QP7.164]=2 | ValueOf[QP7.164]=3] *INCLUDE DummyQP8 [164]
#*IF [ValueOf[QP7.165]=2 | ValueOf[QP7.165]=3] *INCLUDE DummyQP8 [165]
#*IF [ValueOf[QP7.398]=2 | ValueOf[QP7.398]=3] *INCLUDE DummyQP8 [398]
#*IF [ValueOf[QP7.166]=2 | ValueOf[QP7.166]=3] *INCLUDE DummyQP8 [166]
#*IF [ValueOf[QP7.167]=2 | ValueOf[QP7.167]=3] *INCLUDE DummyQP8 [167]
#*IF [ValueOf[QP7.168]=2 | ValueOf[QP7.168]=3] *INCLUDE DummyQP8 [168]
#*IF [ValueOf[QP7.169]=2 | ValueOf[QP7.169]=3] *INCLUDE DummyQP8 [169]
#*IF [ValueOf[QP7.170]=2 | ValueOf[QP7.170]=3] *INCLUDE DummyQP8 [170]
#*IF [ValueOf[QP7.171]=2 | ValueOf[QP7.171]=3] *INCLUDE DummyQP8 [171]
#*IF [ValueOf[QP7.172]=2 | ValueOf[QP7.172]=3] *INCLUDE DummyQP8 [172]
#*IF [ValueOf[QP7.173]=2 | ValueOf[QP7.173]=3] *INCLUDE DummyQP8 [173]
#*IF [ValueOf[QP7.174]=2 | ValueOf[QP7.174]=3] *INCLUDE DummyQP8 [174]
#*IF [ValueOf[QP7.175]=2 | ValueOf[QP7.175]=3] *INCLUDE DummyQP8 [175]
#*IF [ValueOf[QP7.176]=2 | ValueOf[QP7.176]=3] *INCLUDE DummyQP8 [176]
#*IF [ValueOf[QP7.177]=2 | ValueOf[QP7.177]=3] *INCLUDE DummyQP8 [177]
#*IF [ValueOf[QP7.178]=2 | ValueOf[QP7.178]=3] *INCLUDE DummyQP8 [178]
#*IF [ValueOf[QP7.179]=2 | ValueOf[QP7.179]=3] *INCLUDE DummyQP8 [179]
#*IF [ValueOf[QP7.180]=2 | ValueOf[QP7.180]=3] *INCLUDE DummyQP8 [180]
#*IF [ValueOf[QP7.181]=2 | ValueOf[QP7.181]=3] *INCLUDE DummyQP8 [181]
#*IF [ValueOf[QP7.182]=2 | ValueOf[QP7.182]=3] *INCLUDE DummyQP8 [182]
#*IF [ValueOf[QP7.183]=2 | ValueOf[QP7.183]=3] *INCLUDE DummyQP8 [183]
#*IF [ValueOf[QP7.184]=2 | ValueOf[QP7.184]=3] *INCLUDE DummyQP8 [184]
#*IF [ValueOf[QP7.185]=2 | ValueOf[QP7.185]=3] *INCLUDE DummyQP8 [185]
#*IF [ValueOf[QP7.498]=2 | ValueOf[QP7.498]=3] *INCLUDE DummyQP8 [498]
#*IF [ValueOf[QP7.186]=2 | ValueOf[QP7.186]=3] *INCLUDE DummyQP8 [186]
#*IF [ValueOf[QP7.187]=2 | ValueOf[QP7.187]=3] *INCLUDE DummyQP8 [187]
#*IF [ValueOf[QP7.188]=2 | ValueOf[QP7.188]=3] *INCLUDE DummyQP8 [188]
#*IF [ValueOf[QP7.189]=2 | ValueOf[QP7.189]=3] *INCLUDE DummyQP8 [189]
#*IF [ValueOf[QP7.190]=2 | ValueOf[QP7.190]=3] *INCLUDE DummyQP8 [190]
#*IF [ValueOf[QP7.191]=2 | ValueOf[QP7.191]=3] *INCLUDE DummyQP8 [191]
#*IF [ValueOf[QP7.192]=2 | ValueOf[QP7.192]=3] *INCLUDE DummyQP8 [192]
#*IF [ValueOf[QP7.193]=2 | ValueOf[QP7.193]=3] *INCLUDE DummyQP8 [193]
#*IF [ValueOf[QP7.194]=2 | ValueOf[QP7.194]=3] *INCLUDE DummyQP8 [194]
#*IF [ValueOf[QP7.195]=2 | ValueOf[QP7.195]=3] *INCLUDE DummyQP8 [195]
#*IF [ValueOf[QP7.196]=2 | ValueOf[QP7.196]=3] *INCLUDE DummyQP8 [196]
#*IF [ValueOf[QP7.197]=2 | ValueOf[QP7.197]=3] *INCLUDE DummyQP8 [197]
#*IF [ValueOf[QP7.198]=2 | ValueOf[QP7.198]=3] *INCLUDE DummyQP8 [198]
#*IF [ValueOf[QP7.199]=2 | ValueOf[QP7.199]=3] *INCLUDE DummyQP8 [199]
#*IF [ValueOf[QP7.200]=2 | ValueOf[QP7.200]=3] *INCLUDE DummyQP8 [200]
#*IF [ValueOf[QP7.201]=2 | ValueOf[QP7.201]=3] *INCLUDE DummyQP8 [201]
#*IF [ValueOf[QP7.202]=2 | ValueOf[QP7.202]=3] *INCLUDE DummyQP8 [202]
#*IF [ValueOf[QP7.203]=2 | ValueOf[QP7.203]=3] *INCLUDE DummyQP8 [203]
#*IF [ValueOf[QP7.204]=2 | ValueOf[QP7.204]=3] *INCLUDE DummyQP8 [204]
#*IF [ValueOf[QP7.205]=2 | ValueOf[QP7.205]=3] *INCLUDE DummyQP8 [205]
#*IF [ValueOf[QP7.206]=2 | ValueOf[QP7.206]=3] *INCLUDE DummyQP8 [206]
#*IF [ValueOf[QP7.207]=2 | ValueOf[QP7.207]=3] *INCLUDE DummyQP8 [207]
#*IF [ValueOf[QP7.208]=2 | ValueOf[QP7.208]=3] *INCLUDE DummyQP8 [208]
#*IF [ValueOf[QP7.209]=2 | ValueOf[QP7.209]=3] *INCLUDE DummyQP8 [209]
#*IF [ValueOf[QP7.210]=2 | ValueOf[QP7.210]=3] *INCLUDE DummyQP8 [210]
#*IF [ValueOf[QP7.211]=2 | ValueOf[QP7.211]=3] *INCLUDE DummyQP8 [211]
#*IF [ValueOf[QP7.598]=2 | ValueOf[QP7.598]=3] *INCLUDE DummyQP8 [598]
#*IF [ValueOf[QP7.212]=2 | ValueOf[QP7.212]=3] *INCLUDE DummyQP8 [212]
#*IF [ValueOf[QP7.213]=2 | ValueOf[QP7.213]=3] *INCLUDE DummyQP8 [213]
#*IF [ValueOf[QP7.214]=2 | ValueOf[QP7.214]=3] *INCLUDE DummyQP8 [214]
#*IF [ValueOf[QP7.698]=2 | ValueOf[QP7.698]=3] *INCLUDE DummyQP8 [698]
#*IF [ValueOf[QP7.215]=2 | ValueOf[QP7.215]=3] *INCLUDE DummyQP8 [215]
#*IF [ValueOf[QP7.216]=2 | ValueOf[QP7.216]=3] *INCLUDE DummyQP8 [216]
#*IF [ValueOf[QP7.217]=2 | ValueOf[QP7.217]=3] *INCLUDE DummyQP8 [217]
#*IF [ValueOf[QP7.218]=2 | ValueOf[QP7.218]=3] *INCLUDE DummyQP8 [218]
#*IF [ValueOf[QP7.219]=2 | ValueOf[QP7.219]=3] *INCLUDE DummyQP8 [219]
#*IF [ValueOf[QP7.220]=2 | ValueOf[QP7.220]=3] *INCLUDE DummyQP8 [220]
#*IF [ValueOf[QP7.798]=2 | ValueOf[QP7.798]=3] *INCLUDE DummyQP8 [798]
#*IF [ValueOf[QP7.221]=2 | ValueOf[QP7.221]=3] *INCLUDE DummyQP8 [221]
#*IF [ValueOf[QP7.222]=2 | ValueOf[QP7.222]=3] *INCLUDE DummyQP8 [222]
#*IF [ValueOf[QP7.223]=2 | ValueOf[QP7.223]=3] *INCLUDE DummyQP8 [223]
#*IF [ValueOf[QP7.224]=2 | ValueOf[QP7.224]=3] *INCLUDE DummyQP8 [224]
#*IF [ValueOf[QP7.225]=2 | ValueOf[QP7.225]=3] *INCLUDE DummyQP8 [225]
#*IF [ValueOf[QP7.226]=2 | ValueOf[QP7.226]=3] *INCLUDE DummyQP8 [226]
#*IF [ValueOf[QP7.227]=2 | ValueOf[QP7.227]=3] *INCLUDE DummyQP8 [227]
#*IF [ValueOf[QP7.228]=2 | ValueOf[QP7.228]=3] *INCLUDE DummyQP8 [228]
#*IF [ValueOf[QP7.898]=2 | ValueOf[QP7.898]=3] *INCLUDE DummyQP8 [898]
#*IF [ValueOf[QP7.301]=2 | ValueOf[QP7.301]=3] *INCLUDE DummyQP8 [301]
#*IF [ValueOf[QP7.302]=2 | ValueOf[QP7.302]=3] *INCLUDE DummyQP8 [302]
#*IF [ValueOf[QP7.303]=2 | ValueOf[QP7.303]=3] *INCLUDE DummyQP8 [303]
#*IF [ValueOf[QP7.311]=2 | ValueOf[QP7.311]=3] *INCLUDE DummyQP8 [311]
#*IF [ValueOf[QP7.312]=2 | ValueOf[QP7.312]=3] *INCLUDE DummyQP8 [312]
#*IF [ValueOf[QP7.313]=2 | ValueOf[QP7.313]=3] *INCLUDE DummyQP8 [313]
#*IF [ValueOf[QP7.321]=2 | ValueOf[QP7.321]=3] *INCLUDE DummyQP8 [321]
#*IF [ValueOf[QP7.322]=2 | ValueOf[QP7.322]=3] *INCLUDE DummyQP8 [322]
#*IF [ValueOf[QP7.323]=2 | ValueOf[QP7.323]=3] *INCLUDE DummyQP8 [323]
#*IF [ValueOf[QP7.331]=2 | ValueOf[QP7.331]=3] *INCLUDE DummyQP8 [331]
#*IF [ValueOf[QP7.332]=2 | ValueOf[QP7.332]=3] *INCLUDE DummyQP8 [332]
#*IF [ValueOf[QP7.333]=2 | ValueOf[QP7.333]=3] *INCLUDE DummyQP8 [333]
#*IF [ValueOf[QP7.341]=2 | ValueOf[QP7.341]=3] *INCLUDE DummyQP8 [341]
#*IF [ValueOf[QP7.342]=2 | ValueOf[QP7.342]=3] *INCLUDE DummyQP8 [342]
#*IF [ValueOf[QP7.343]=2 | ValueOf[QP7.343]=3] *INCLUDE DummyQP8 [343]
#*IF [ValueOf[QP7.351]=2 | ValueOf[QP7.351]=3] *INCLUDE DummyQP8 [351]
#*IF [ValueOf[QP7.352]=2 | ValueOf[QP7.352]=3] *INCLUDE DummyQP8 [352]
#*IF [ValueOf[QP7.353]=2 | ValueOf[QP7.353]=3] *INCLUDE DummyQP8 [353]
#*IF [ValueOf[QP7.361]=2 | ValueOf[QP7.361]=3] *INCLUDE DummyQP8 [361]
#*IF [ValueOf[QP7.362]=2 | ValueOf[QP7.362]=3] *INCLUDE DummyQP8 [362]
#*IF [ValueOf[QP7.363]=2 | ValueOf[QP7.363]=3] *INCLUDE DummyQP8 [363]
#*IF [ValueOf[QP7.371]=2 | ValueOf[QP7.371]=3] *INCLUDE DummyQP8 [371]
#*IF [ValueOf[QP7.372]=2 | ValueOf[QP7.372]=3] *INCLUDE DummyQP8 [372]
#*IF [ValueOf[QP7.373]=2 | ValueOf[QP7.373]=3] *INCLUDE DummyQP8 [373]
#*IF [ValueOf[QP7.381]=2 | ValueOf[QP7.381]=3] *INCLUDE DummyQP8 [381]
#*IF [ValueOf[QP7.382]=2 | ValueOf[QP7.382]=3] *INCLUDE DummyQP8 [382]
#*IF [ValueOf[QP7.383]=2 | ValueOf[QP7.383]=3] *INCLUDE DummyQP8 [383]
#*IF [ValueOf[QP7.391]=2 | ValueOf[QP7.391]=3] *INCLUDE DummyQP8 [391]
#*IF [ValueOf[QP7.392]=2 | ValueOf[QP7.392]=3] *INCLUDE DummyQP8 [392]
#*IF [ValueOf[QP7.393]=2 | ValueOf[QP7.393]=3] *INCLUDE DummyQP8 [393]
#*IF [ValueOf[QP7.401]=2 | ValueOf[QP7.401]=3] *INCLUDE DummyQP8 [401]
#*IF [ValueOf[QP7.402]=2 | ValueOf[QP7.402]=3] *INCLUDE DummyQP8 [402]
#*IF [ValueOf[QP7.403]=2 | ValueOf[QP7.403]=3] *INCLUDE DummyQP8 [403]
#*IF [ValueOf[QP7.411]=2 | ValueOf[QP7.411]=3] *INCLUDE DummyQP8 [411]
#*IF [ValueOf[QP7.412]=2 | ValueOf[QP7.412]=3] *INCLUDE DummyQP8 [412]
#*IF [ValueOf[QP7.413]=2 | ValueOf[QP7.413]=3] *INCLUDE DummyQP8 [413]
#*IF [ValueOf[QP7.421]=2 | ValueOf[QP7.421]=3] *INCLUDE DummyQP8 [421]
#*IF [ValueOf[QP7.422]=2 | ValueOf[QP7.422]=3] *INCLUDE DummyQP8 [422]
#*IF [ValueOf[QP7.423]=2 | ValueOf[QP7.423]=3] *INCLUDE DummyQP8 [423]
#*IF [ValueOf[QP7.431]=2 | ValueOf[QP7.431]=3] *INCLUDE DummyQP8 [431]
#*IF [ValueOf[QP7.432]=2 | ValueOf[QP7.432]=3] *INCLUDE DummyQP8 [432]
#*IF [ValueOf[QP7.433]=2 | ValueOf[QP7.433]=3] *INCLUDE DummyQP8 [433]


*GRIDLIST "GridQP8"
1:Bills and utilities
2:Dining
3:E-commerce
4:Electronics
5:Entertainment
6:Fuel
7:Groceries
8:Hospitals
9:Travel
10:Insurance Premium
98:Any other *OPEN


*QUESTION QP8 *GRIDMR *USEGRIDLIST "GridQP8" *INCLUDE [CardBrand] *IF [NumberOfResponse[CardBrand]>0] *HORIZONTAL *IMGADJBY 400
QP8. What types of purchases do you mostly make with each of your credit card? You can choose multiple options for each cards if you do that way. [MA]
*USELIST "CCListImage"

*GRIDLIST "GridQP9"
0:Completely dissatisfied
1:1 
2:2 
3:3 
4:4 
5:5 
6:6 
7:7 
8:8 
9:9 
10:Completely satisfied

# *SHOWASFORM
*QUESTION QP9 *GRIDSR *USEGRIDLIST "GridQP9" *INCLUDE [CardBrand]
QP9. Overall How satisfied with each of these credit cards on a scale of 0-10. Where 0 means not at all satisfied & 10 means extremely satisfied [SA FOR EACH BRAND]
How satisfied are you with ---------
*USELIST "CCListImage"





######################################MEDIA####################################

*QUESTION QM1 *MR
[SHOW SCREEN]
QM1. Can you tell us how do you usually spend your leisure time/free time on a weekday?
1:Listening to Music
2:Watching TV/OTT
3:Shopping online
4:Visiting Social Media sites
5:Cooking at home  
6:Partying outside home
7:Socializing  with Inviting Guests over 
#8:Inviting Guests over
9:Going on drives  
10:Shopping offline i.e. in shops, malls etc.
11:Entertainment/movies
12:Sports & Games
13:Wellness & Spa
98:Others *OPEN

*QUESTION QM2 *MR
[SHOW SCREEN]
QM2. Can you tell us how do you usually spend your leisure time on a weekend?
1:Listening to Music
2:Watching TV/OTT
3:Shopping online
4:Visiting Social Media sites
5:Cooking at home  
6:Partying outside home
7:Socializing  with Inviting Guests over 
#8:Inviting Guests over
9:Going on drives  
10:Shopping offline i.e. in shops, malls etc.
11:Entertainment/movies
12:Sports & Games
13:Wellness & Spa
98:Others *OPEN

*QUESTION QM3 *SR
QM3. Can you tell us how frequently do you go on trips to different destinations within India? It could be business or leisure trips. [SA]
1:Few times a month
2:Once a month
3:Once in two months
4:2-3 times a year
5:Once every year
6:Once in two years  
7:Less frequently
99:Never

*QUESTION QM4 *SR
QM4. Can you tell us how frequently do you go on trips to different destinations out of India ? It could be business or leisure trips. [SA]
#1:Once a month
#2:Once in two months
#3:Once every six months
4:2-3 times a year
5:Once every year
6:Once in two years  
7:Less frequently
99:Never

*STARTREC "MQ7MQ8"

*QUESTION MQ5 *SR
MQ5. Which one of the following best describes your marital status? [SA]
1:Single   
2:Married (with or without kids) 
3:Divorced / Separated   

*QUESTION MQ6 *SR
MQ6. Is your family a nuclear family or a joint family? [SA]
1:Nuclear family
2:Joint family

*QUESTION MQ7 *SR
[SHOW CARD]<br>
MQ7. Now Please mention your <b>Personal Annual income</b>? 
Please include income from all sources for yourself  Please include bonuses, interest on financial investments, dividends, house rent, pension, etc.
1:Less than 5 Lakh
2:Rs.5.1-7 Lakh
3:Rs.7.1- 10 Lakh
4:Rs.10.1-12.5 Lakh
5:Rs.12.6- 15 Lakh
6:Rs.15.1-20 Lakh
7:Rs.20.1- 25 Lakh
8:Rs.25.1-30 Lakh
9:Rs.30.1- 35 Lakh
10:Rs.35.1-40 Lakh
11:Rs.40.1- 50 Lakh
12:Rs.50.1-60 Lakh
13:Rs.60.1- 70 Lakh
14:Rs.70.1- 80 Lakh
15:Rs.80.1 Lakh - 1 Cr
16:Rs.1.1 Cr.- 1.5 Cr
17:More than Rs. 1.5 CR
99:Refused  (DO NOT SHOW)

*QUESTION MPI *SR *DUMMY2
Personal Annual income
#1:Less Than 20 Lakhs
2:Rs. 20-50 Lakhs (20%)
3:Rs. 50 Lakhs & above (15%)
#99:Refused  (DO NOT SHOW)

*IF [ValueOf[MQ7]>6 & ValueOf[MQ7]<12] *INCLUDE MPI [2]
*IF [ValueOf[MQ7]>11 & ValueOf[MQ7]<18] *INCLUDE MPI [3] 

*QUESTION MQ8 *SR
[SHOW CARD]<br>
MQ8. Now Please mention your <b>Annual Household income</b>? 
Please include income from all sources. Please include bonuses, interest on financial investments, dividends, house rent, pension, etc.
1:Less than 5 Lakh
2:Rs.5.1-7 Lakh
3:Rs.7.1- 10 Lakh
4:Rs.10.1-12.5 Lakh
5:Rs.12.6- 15 Lakh
6:Rs.15.1-20 Lakh
7:Rs.20.1- 25 Lakh
8:Rs.25.1-30 Lakh
9:Rs.30.1- 35 Lakh
10:Rs.35.1-40 Lakh
11:Rs.40.1- 50 Lakh
12:Rs.50.1-60 Lakh
13:Rs.60.1- 70 Lakh
14:Rs.70.1- 80 Lakh
15:Rs.80.1 Lakh - 1 Cr
16:Rs.1.1 Cr.- 1.5 Cr
17:More than Rs. 1.5 CR
99:Refused  (DO NOT SHOW)


*IF [MQ7!=99 & MQ8!=99 & ValueOf[MQ8]<ValueOf[MQ7]] *MSG "Anual Household Income {MQ8} should not be less than Anual Personal Income {MQ7}"

*STARTREC "QZ1"

*QUESTION QZ1 *SR
QZ1. To improve the service quality, can we share your contact details with our clients. Rest assured that it will not be used for sales or promotion purposes. [SA]
1:Yes
2:No

*ENDREC

*QUESTION QZ2 *SR *IF [Q7=2]
QZ2. Now I would like to ask you something about the Chief Wage Earner of the family. 
By Chief Wage Earner, I mean that person in your household who contributes the maximum to the monthly household expenditure.
<br>What is his/her Occupation?  [SA]
1:Unskilled Worker
2:Skilled Worker
3:Petty Traders
4:Shop Owners
5:Businessman/Industrialist (No Employees)
6:Businessman/Industrialist (1 – 9 Employees)
7:Businessman/Industrialist (10+ Employees)
8:Self Employed Professional
9:Clerk / Salesman
10:Supervisory Level
11:Officer / Executive – Junior
12:Officer / Executive – Middle /Senior
13:Owner Farmer
14:Leased Farmer
98:Others *OPEN

*IF [Q7=1] *INCLUDE QZ2 Q10

*QUESTION QZ4 *SR 
[SHOW SCREEN]
<br>QZ4. Among men above 21 years of age, who live in this household, what is the highest that anyone has studied? If no male adult over 21, ask for males in ‘18 to 20 years' age group. If no male adult over 18, choose accordingly.
1:No male adult
2:No formal education
3:Upto Class 5 standard                      
4:Class 6 – 9th standard
5:Class 10 -14 standard
6:Degree Regular (e.g. B.A., B.Sc., B.Com., M.A., M.Sc., M.Com, BBA, BCA)                                
7:Degree Professional (e.g. B.E., B.Tech., MTech., C.A., M.B.B.S., L.L.B., MBA, MCA, MD, PhD)                 

*QUESTION QZ5 *SR 
[SHOW SCREEN]
<br>QZ5. Among women above 21 years of age, who live in this household, what is the highest that anyone has studied? If no female adult over 21, ask for females in ‘18 to 20 years' age group. If no female adult over 18, choose accordingly.
1:No female adult
2:No formal education
3:Upto Class 5 standard                      
4:Class 6 – 9th standard
5:Class 10 -14 standard
6:Degree Regular (e.g. B.A., B.Sc., B.Com., M.A., M.Sc., M.Com, BBA, BCA)                                
7:Degree Professional (e.g. B.E., B.Tech., MTech., C.A., M.B.B.S., L.L.B., MBA, MCA, MD, PhD)      

*ENDREC           

*QUESTION OCC *SR *DUMMY2
<b>OCCUPATION CLASSIFICATION</b>
1:Labour
2:Farmer
3:Worker
4:Trader
5:Clerical/Sales/Supervisory
6:Managerial/professional

*IF [QZ2=1] *INCLUDE OCC [1]
*IF [QZ2=13 | QZ2=14] *INCLUDE OCC [2]
*IF [QZ2=2] *INCLUDE OCC [3]
*IF [QZ2=3 | QZ2=4 | QZ2=5 | QZ2=6] *INCLUDE OCC [4]
*IF [QZ2=9 | QZ2=10] *INCLUDE OCC [5]
*IF [QZ2=7 | QZ2=8 | QZ2=11 | QZ2=12] *INCLUDE OCC [6]

*QUESTION ISEC *SR *DUMMY2
ISEC Classfination
1:NCCS A1
2:NCCS A2
3:NCCS A3
4:NCCS B1
5:NCCS B2
6:NCCS C1
7:NCCS C2
8:NCCS D1
9:NCCS D2
10:NCCS E1
11:NCCS E2
12:NCCS E3

*IF [OCC=1 & QZ4=1 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=1 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=1 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=1 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=1 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=1 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=1 & QZ5=7] *INCLUDE ISEC [9]

*IF [OCC=1 & QZ4=2 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=2 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=2 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=2 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=2 & QZ5=5] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=2 & QZ5=6] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=2 & QZ5=7] *INCLUDE ISEC [10]

*IF [OCC=1 & QZ4=3 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=3 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=3 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=3 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=3 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=3 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=3 & QZ5=7] *INCLUDE ISEC [9]

*IF [OCC=1 & QZ4=4 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=1 & QZ4=4 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=4 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=4 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=4 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=4 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=4 & QZ5=7] *INCLUDE ISEC [9]

*IF [OCC=1 & QZ4=5 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=5 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=1 & QZ4=5 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=5 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=5 & QZ5=5] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=5 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=1 & QZ4=5 & QZ5=7] *INCLUDE ISEC [7]

*IF [OCC=1 & QZ4=6 & QZ5=1] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=6 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=6 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=6 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=6 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=1 & QZ4=6 & QZ5=6] *INCLUDE ISEC [7]
*IF [OCC=1 & QZ4=6 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=1 & QZ4=7 & QZ5=1] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=7 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=1 & QZ4=7 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=1 & QZ4=7 & QZ5=4] *INCLUDE ISEC [8]
*IF [OCC=1 & QZ4=7 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=1 & QZ4=7 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=1 & QZ4=7 & QZ5=7] *INCLUDE ISEC [6]


*IF [OCC=2 & QZ4=1 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=1 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=1 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=1 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=1 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=1 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=1 & QZ5=7] *INCLUDE ISEC [9]

*IF [OCC=2 & QZ4=2 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=2 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=2 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=2 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=2 & QZ5=5] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=2 & QZ5=6] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=2 & QZ5=7] *INCLUDE ISEC [10]

*IF [OCC=2 & QZ4=3 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=3 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=3 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=3 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=3 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=3 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=3 & QZ5=7] *INCLUDE ISEC [9]

*IF [OCC=2 & QZ4=4 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=2 & QZ4=4 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=4 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=4 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=4 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=4 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=4 & QZ5=7] *INCLUDE ISEC [9]

*IF [OCC=2 & QZ4=5 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=5 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=2 & QZ4=5 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=5 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=5 & QZ5=5] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=5 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=2 & QZ4=5 & QZ5=7] *INCLUDE ISEC [7]

*IF [OCC=2 & QZ4=6 & QZ5=1] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=6 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=6 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=6 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=6 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=2 & QZ4=6 & QZ5=6] *INCLUDE ISEC [7]
*IF [OCC=2 & QZ4=6 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=2 & QZ4=7 & QZ5=1] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=7 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=2 & QZ4=7 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=2 & QZ4=7 & QZ5=4] *INCLUDE ISEC [8]
*IF [OCC=2 & QZ4=7 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=2 & QZ4=7 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=2 & QZ4=7 & QZ5=7] *INCLUDE ISEC [5]


*IF [OCC=3 & QZ4=1 & QZ5=1] *INCLUDE ISEC [12]
*IF [OCC=3 & QZ4=1 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=3 & QZ4=1 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=1 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=1 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=1 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=1 & QZ5=7] *INCLUDE ISEC [8]

*IF [OCC=3 & QZ4=2 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=2 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=2 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=2 & QZ5=4] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=2 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=2 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=2 & QZ5=7] *INCLUDE ISEC [8]

*IF [OCC=3 & QZ4=3 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=3 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=3 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=3 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=3 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=3 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=3 & QZ5=7] *INCLUDE ISEC [7]

*IF [OCC=3 & QZ4=4 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=4 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=3 & QZ4=4 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=4 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=4 & QZ5=5] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=4 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=4 & QZ5=7] *INCLUDE ISEC [7]

*IF [OCC=3 & QZ4=5 & QZ5=1] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=5 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=3 & QZ4=5 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=5 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=5 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=5 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=3 & QZ4=5 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=3 & QZ4=6 & QZ5=1] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=6 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=6 & QZ5=3] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=6 & QZ5=4] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=6 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=3 & QZ4=6 & QZ5=6] *INCLUDE ISEC [5]
*IF [OCC=3 & QZ4=6 & QZ5=7] *INCLUDE ISEC [4]

*IF [OCC=3 & QZ4=7 & QZ5=1] *INCLUDE ISEC [8]
*IF [OCC=3 & QZ4=7 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=3 & QZ4=7 & QZ5=3] *INCLUDE ISEC [7]
*IF [OCC=3 & QZ4=7 & QZ5=4] *INCLUDE ISEC [7]
*IF [OCC=3 & QZ4=7 & QZ5=5] *INCLUDE ISEC [5]
*IF [OCC=3 & QZ4=7 & QZ5=6] *INCLUDE ISEC [3]
*IF [OCC=3 & QZ4=7 & QZ5=7] *INCLUDE ISEC [3]


*IF [OCC=4 & QZ4=1 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=1 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=4 & QZ4=1 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=1 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=4 & QZ4=1 & QZ5=5] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=1 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=4 & QZ4=1 & QZ5=7] *INCLUDE ISEC [5]

*IF [OCC=4 & QZ4=2 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=2 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=2 & QZ5=3] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=2 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=4 & QZ4=2 & QZ5=5] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=2 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=2 & QZ5=7] *INCLUDE ISEC [8]

*IF [OCC=4 & QZ4=3 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=3 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=3 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=4 & QZ4=3 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=3 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=3 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=3 & QZ5=7] *INCLUDE ISEC [7]

*IF [OCC=4 & QZ4=4 & QZ5=1] *INCLUDE ISEC [10]
*IF [OCC=4 & QZ4=4 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=4 & QZ4=4 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=4 & QZ4=4 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=4 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=4 & QZ5=6] *INCLUDE ISEC [7]
*IF [OCC=4 & QZ4=4 & QZ5=7] *INCLUDE ISEC [5]

*IF [OCC=4 & QZ4=5 & QZ5=1] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=5 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=4 & QZ4=5 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=5 & QZ5=4] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=5 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=4 & QZ4=5 & QZ5=6] *INCLUDE ISEC [5]
*IF [OCC=4 & QZ4=5 & QZ5=7] *INCLUDE ISEC [4]

*IF [OCC=4 & QZ4=6 & QZ5=1] *INCLUDE ISEC [7]
*IF [OCC=4 & QZ4=6 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=4 & QZ4=6 & QZ5=3] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=6 & QZ5=4] *INCLUDE ISEC [7]
*IF [OCC=4 & QZ4=6 & QZ5=5] *INCLUDE ISEC [6]
*IF [OCC=4 & QZ4=6 & QZ5=6] *INCLUDE ISEC [3]
*IF [OCC=4 & QZ4=6 & QZ5=7] *INCLUDE ISEC [2]

*IF [OCC=4 & QZ4=7 & QZ5=1] *INCLUDE ISEC [6]
*IF [OCC=4 & QZ4=7 & QZ5=2] *INCLUDE ISEC [8]
*IF [OCC=4 & QZ4=7 & QZ5=3] *INCLUDE ISEC [6]
*IF [OCC=4 & QZ4=7 & QZ5=4] *INCLUDE ISEC [6]
*IF [OCC=4 & QZ4=7 & QZ5=5] *INCLUDE ISEC [4]
*IF [OCC=4 & QZ4=7 & QZ5=6] *INCLUDE ISEC [2]
*IF [OCC=4 & QZ4=7 & QZ5=7] *INCLUDE ISEC [2]


*IF [OCC=5 & QZ4=1 & QZ5=1] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=1 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=5 & QZ4=1 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=1 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=1 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=1 & QZ5=6] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=1 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=5 & QZ4=2 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=5 & QZ4=2 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=5 & QZ4=2 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=2 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=2 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=2 & QZ5=6] *INCLUDE ISEC [9]
*IF [OCC=5 & QZ4=2 & QZ5=7] *INCLUDE ISEC [8]

*IF [OCC=5 & QZ4=3 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=5 & QZ4=3 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=5 & QZ4=3 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=3 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=5 & QZ4=3 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=3 & QZ5=6] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=3 & QZ5=7] *INCLUDE ISEC [7]

*IF [OCC=5 & QZ4=4 & QZ5=1] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=4 & QZ5=2] *INCLUDE ISEC [10]
*IF [OCC=5 & QZ4=4 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=5 & QZ4=4 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=5 & QZ4=4 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=4 & QZ5=6] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=4 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=5 & QZ4=5 & QZ5=1] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=5 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=5 & QZ4=5 & QZ5=3] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=5 & QZ5=4] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=5 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=5 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=5 & QZ4=5 & QZ5=7] *INCLUDE ISEC [4]

*IF [OCC=5 & QZ4=6 & QZ5=1] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=6 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=5 & QZ4=6 & QZ5=3] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=6 & QZ5=4] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=6 & QZ5=5] *INCLUDE ISEC [6]
*IF [OCC=5 & QZ4=6 & QZ5=6] *INCLUDE ISEC [4]
*IF [OCC=5 & QZ4=6 & QZ5=7] *INCLUDE ISEC [3]

*IF [OCC=5 & QZ4=7 & QZ5=1] *INCLUDE ISEC [6]
*IF [OCC=5 & QZ4=7 & QZ5=2] *INCLUDE ISEC [8]
*IF [OCC=5 & QZ4=7 & QZ5=3] *INCLUDE ISEC [7]
*IF [OCC=5 & QZ4=7 & QZ5=4] *INCLUDE ISEC [6]
*IF [OCC=5 & QZ4=7 & QZ5=5] *INCLUDE ISEC [4]
*IF [OCC=5 & QZ4=7 & QZ5=6] *INCLUDE ISEC [2]
*IF [OCC=5 & QZ4=7 & QZ5=7] *INCLUDE ISEC [2]


*IF [OCC=6 & QZ4=1 & QZ5=1] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=1 & QZ5=2] *INCLUDE ISEC [12]
*IF [OCC=6 & QZ4=1 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=1 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=1 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=6 & QZ4=1 & QZ5=6] *INCLUDE ISEC [5]
*IF [OCC=6 & QZ4=1 & QZ5=7] *INCLUDE ISEC [5]

*IF [OCC=6 & QZ4=2 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=6 & QZ4=2 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=6 & QZ4=2 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=2 & QZ5=4] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=2 & QZ5=5] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=2 & QZ5=6] *INCLUDE ISEC [8]
*IF [OCC=6 & QZ4=2 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=6 & QZ4=3 & QZ5=1] *INCLUDE ISEC [11]
*IF [OCC=6 & QZ4=3 & QZ5=2] *INCLUDE ISEC [11]
*IF [OCC=6 & QZ4=3 & QZ5=3] *INCLUDE ISEC [10]
*IF [OCC=6 & QZ4=3 & QZ5=4] *INCLUDE ISEC [9]
*IF [OCC=6 & QZ4=3 & QZ5=5] *INCLUDE ISEC [8]
*IF [OCC=6 & QZ4=3 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=6 & QZ4=3 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=6 & QZ4=4 & QZ5=1] *INCLUDE ISEC [9]
*IF [OCC=6 & QZ4=4 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=6 & QZ4=4 & QZ5=3] *INCLUDE ISEC [9]
*IF [OCC=6 & QZ4=4 & QZ5=4] *INCLUDE ISEC [8]
*IF [OCC=6 & QZ4=4 & QZ5=5] *INCLUDE ISEC [7]
*IF [OCC=6 & QZ4=4 & QZ5=6] *INCLUDE ISEC [6]
*IF [OCC=6 & QZ4=4 & QZ5=7] *INCLUDE ISEC [6]

*IF [OCC=6 & QZ4=5 & QZ5=1] *INCLUDE ISEC [7]
*IF [OCC=6 & QZ4=5 & QZ5=2] *INCLUDE ISEC [9]
*IF [OCC=6 & QZ4=5 & QZ5=3] *INCLUDE ISEC [8]
*IF [OCC=6 & QZ4=5 & QZ5=4] *INCLUDE ISEC [7]
*IF [OCC=6 & QZ4=5 & QZ5=5] *INCLUDE ISEC [5]
*IF [OCC=6 & QZ4=5 & QZ5=6] *INCLUDE ISEC [3]
*IF [OCC=6 & QZ4=5 & QZ5=7] *INCLUDE ISEC [3]

*IF [OCC=6 & QZ4=6 & QZ5=1] *INCLUDE ISEC [6]
*IF [OCC=6 & QZ4=6 & QZ5=2] *INCLUDE ISEC [8]
*IF [OCC=6 & QZ4=6 & QZ5=3] *INCLUDE ISEC [7]
*IF [OCC=6 & QZ4=6 & QZ5=4] *INCLUDE ISEC [6]
*IF [OCC=6 & QZ4=6 & QZ5=5] *INCLUDE ISEC [4]
*IF [OCC=6 & QZ4=6 & QZ5=6] *INCLUDE ISEC [2]
*IF [OCC=6 & QZ4=6 & QZ5=7] *INCLUDE ISEC [1]

*IF [OCC=6 & QZ4=7 & QZ5=1] *INCLUDE ISEC [5]
*IF [OCC=6 & QZ4=7 & QZ5=2] *INCLUDE ISEC [7]
*IF [OCC=6 & QZ4=7 & QZ5=3] *INCLUDE ISEC [6]
*IF [OCC=6 & QZ4=7 & QZ5=4] *INCLUDE ISEC [5]
*IF [OCC=6 & QZ4=7 & QZ5=5] *INCLUDE ISEC [3]
*IF [OCC=6 & QZ4=7 & QZ5=6] *INCLUDE ISEC [1]
*IF [OCC=6 & QZ4=7 & QZ5=7] *INCLUDE ISEC [1]

############### End Of Main Script Part ##############################

*QUESTION Accom *OPEN *DKCS "No Accompaniment" "99"
Interview Accompanied By

*QUESTION AccomPic *CAPTUREIMAGE *IF [Accom!=99]
Please take a picture/selfi with who is accompanied

*QUESTION FN *END
Interview has been completed successfully.

*QUESTION TN *TERMINATE
Interview has been Terminated.

*END

