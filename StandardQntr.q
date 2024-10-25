#####################################################

Project Name : Standard Script
Project Code : 1000
Script Version : 1.0.0.1
Script Name : SYSISMDB
Scripted By : Md. Ismile Hossain

#####################################################

*LIST "BrandList"
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola


*LIST "Scale"
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange





*QUESTION Q1 *SR
Q1.Thinking only about CARBONATED SOFT DRINKS, which brand comes to your mind first?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
95:Others *OTHERS


*QUESTION Q2 *MR *MIN 1 *MAX 3 *EXCLUDE [Q1]
Q2. Apart from the TOM brand, What other brands do you know of CARBONATED SOFT DRINKS?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
96:Others *OTHERS
97:Others *OTHERS
98:Others *OTHERS
99:None *NMUL


*QUESTION Q3Dummy *MR *DUMMY1
Q3Dummy
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
95:OTHERS
96:OTHERS
97:OTHERS
98:OTHERS

*INCLUDE Q3Dummy [01 TO 05]

*QUESTION Q14 *RANK
Q14. Please Rank the response as per respondent answer order?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola


*INCLUDE Q3Dummy Q1
*INCLUDE Q3Dummy [01;03;04;05]


*EXCLUDE Q3Dummy Q1
*EXCLUDE Q3Dummy [01;03;04;05]

*IF [Q5=1] *GOTO AllAware


*QUESTION Q3 *SR
Q3. Which of the CARBONATED SOFT DRINKS have you heard of?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
95:{Q1}
96:{Q2.96}
97:{Q2.97}
98:{Q2.98}

*IF [Q5=1] *MSG "Mobile Number should be correct"

*QUESTION AllAware *MR *DUMMY1
All Aware Brands
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
95:OTHERS
96:OTHERS
97:OTHERS
98:OTHERS



*QUESTION Q4 *SR *REPORT
Q4. Which of the below CARBONATED SOFT DRINKS have you ever consumed?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
95:{Q1}
96:{Q2.96}
97:{Q2.97}
98:{Q2.98}

*QUESTION Q10 *NUMBER
Q10. Actual Age

*QUESTION Q100 *NUMBER *DKCS "Do not know/Can not say" "99"
Q10. Actual Age

*QUESTION Q11 *ALPHA
Q11. Name of respondent

*QUESTION Q110 *ALPHA *DKCS "Do not know/Can not say" "99"
Q11. Name of respondent

*QUESTION Q5 *SR
Q5. Which of the below CARBONATED SOFT DRINKS have you consumed L3M?
*USELIST "BrandList"
95:{Q1}
96:{Q2.96}
97:{Q2.97}
98:{Q2.98}

*QUESTION Q13 *INFO
This is an infomation type question

*QUESTION Q6 *SR
Q6.Which one is your most often consumed brand, that is you consume more than any other CARBONATED SOFT DRINKS brand?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew
11:Pran UP
12:Mojo
13:Fizz Up
14:Clemon
15:Uro Cola
16:RC Cola
95:{Q1}
96:{Q2.96}
97:{Q2.97}
98:{Q2.98}

*GRIDLIST "MyGridList"
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange

*QUESTION Q12 *GRIDSR *USEGRIDLIST "MyGridList"
Q12.Which one is your most often consumed brand, that is you consume more than any other CARBONATED SOFT DRINKS brand?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew



*QUESTION Q20 *GRIDMR *USEGRIDLIST "MyGridList"
Q20.Which one is your most often consumed brand, that is you consume more than any other CARBONATED SOFT DRINKS brand?
1:Coca-Cola
2:Sprite
3:Fanta
4:Uro Lemon
5:Uro Orange
6:RC Lemon
7:Pepsi
8:7UP
9:Mirinda 
10:Mountain Dew

*QUESTION FN *END
Interview has been completed successfully.

*QUESTION TN *TERMINATE
Interview has been Terminated.

*END

#Q2. Apart from the MOCB brand, is there any other CARBONATED SOFT DRINKS brand which you also consume occasionally?

 
#Q3. Which brand of CARBONATED SOFT DRINKS you have just bought?


#Q3. Which of the CARBONATED SOFT DRINKS have you heard of?