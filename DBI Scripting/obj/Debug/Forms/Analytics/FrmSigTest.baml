   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	   i eMicrosoft.TeamFoundation.Controls, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls%Microsoft.TeamFoundation.Controls.WPF 5   	    DBI Scripting.   (DBI_Scripting.Forms.Analytics.FrmSigTest   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   nWPF^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls     5   	   -   5          �Name$  
frmSigTest��5   t    � Title$ Significance Test��5   	   $	��640��6#   $	��900��60    � WindowStartupLocation$ CenterScreen=�6<    � 
ResizeMode$ NoResize=�6a    � Icon$3 -/DBI Scripting;component/Images/reject_64.png��6w   .��5      � .+�5   
   �� $ ��Prepare Table for Sig Test��6   $
��Left=�67   $��40,3,0,0q�6R   $	��Top=�6d   $	��207��6|   $	��809��6�   .��5      � .+�5	      �� $��Table Excel Path :��6   $
��Left=�65   $��	11,25,0,0q�6P   $	��Top=�6c    J� RenderTransformOrigin$ -3.74,-2.112&�6{   �� 5
      -   5         ��Name$ txtExcelTableFilePath��5
      $
��Left=�69   $��23��6T   $��
109,28,0,0q�6`    �� TextWrapping$
 Wrap=�6t   $	��Top=�6�   $	��599��6�   �� 5      -   5         ��Name$ btnBrowseExcelTableFile��5      $��Browse��6:   $
��Left=�6K   $��
712,30,0,0q�6f   $	��Top=�6z   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5      -   5        $ btnRunPrepareTable��5      $	��Run��65   $
��Left=�6C   $��
680,97,0,0q�6^   $	��Top=�6r   $	��108��6�   $��31��6�   6�   �� 5      -   5        $ btnKillProcess��5      $��Kill Process��61   $
��Left=�6H   $��556,117,0,0q�6c   $	��Top=�6x   $	��108��6�   $��31��6�   6�   �� 5      $��	WrokSheet��6   $
��Left=�6,   $��	66,83,0,0q�6G   $	��Top=�6Z   $ -3.74,-2.112&�6r   $   Xceed.Wpf.Toolkit.CheckListBox  5      -   5        	  Name$	 chkListBoxWorksheet��5   '   $
��Left=�6D   $��70��6_   $��
16,109,0,0q�6k   $	��Top=�6   $	��179��6�   6�   �� 5      $�� ��6   $
��Left=�6%   $��200,110,0,0q�6@   $	��Top=�6U   $��66��6m   $	��185��6y   5      	� 5      -   5        
 	�Name$
 ChkUniqueWithinGroup��5      $��Unique code within group��6<   $
��Left=�6_   $��216,129,0,0q�6z   $	��Top=�6�   $��21��6�   $	��169��6�   	� 5      -   5        $
 ChkUniqueAllGroup��5      $��Unique code all group��69   $
��Left=�6Y   $��216,153,0,0q�6t   $	��Top=�6�   $��21��6�   $	��169��6�   �� 5      -	   5        $ btnClosePrepare��5      $��Close��62   $
��Left=�6B   $��680,140,0,0q�6]   $	��Top=�6r   $	��108��6�   $��31��6�   6�   � 5      -
   5         �Name$ progressBar1��5      $
��Left=�64   $��17��6O   $��	158,4,0,0q�6[   $	��Top=�6n   $	��508��6�    � IsIndeterminate .   � Value 6�   �� *
Progress+6�   �� 5      $��Group Letter File :��6   $
��Left=�66   $��6,57,0,0q�6Q   $	��Top=�6c   $ -3.74,-2.112&�6{   �� 5      -   5        $ txtGroupLetterFilePath��5      $
��Left=�6:   $��23��6U   $��
109,59,0,0q�6a   $
 Wrap=�6u   $	��Top=�6�   $	��599��6�   �� 5      -   5        $ btnBrowseGroupLetterFile��5      $��Browse��6;   $
��Left=�6L   $��
712,61,0,0q�6g   $	��Top=�6{   $��75��6�   $ 8.079,-5.321&�6�   6�   5      5       �� 5!   
   $��Set Sig Test Value��6   $
��Left=�6/   $��
40,211,0,0q�6J   $	��Top=�6^   $	��375��6v   $	��809��6�   .��5"      � .+�5#      �� $��Table Excel Path :��6   $
��Left=�65   $��	11,22,0,0q�6P   $	��Top=�6c   $ -3.74,-2.112&�6{   �� 5$      -   5        $ txtExcelTableSetSigTest��5$      $
��Left=�6;   $��23��6V   $��
109,25,0,0q�6b   $
 Wrap=�6v   $	��Top=�6�   $	��599��6�   �� 5%      -   5        $ btnBrowseSetSigTest��5%      $��Browse��66   $
��Left=�6G   $��
712,27,0,0q�6b   $	��Top=�6v   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5&      -   5        $ btnRunSetSigTest��5&      $	��Run��63   $
��Left=�6A   $��643,248,0,0q�6\   $	��Top=�6q   $	��108��6�   $��31��6�   6�   �� 5'      $��	WrokSheet��6   $
��Left=�6,   $��
469,79,0,0q�6G   $	��Top=�6[   $ -3.74,-2.112&�6s     5(      -   5        $	 chkListBoxWorksheetSetSig��5(   '   $
��Left=�6J   $��79��6e   $��419,106,0,0q�6q   $	��Top=�6�   $	��179��6�   6�   �� 5)      $��Group Def File :��6   $
��Left=�63   $��	11,53,0,0q�6N   $	��Top=�6a   $ -3.74,-2.112&�6y   �� 5*      -   5        $ txtGroupFile��5*      $
��Left=�60   $��23��6K   $��
109,56,0,0q�6W   $
 Wrap=�6k   $	��Top=�6   $	��599��6�   �� 5+      -   5        $ btnBrowseGroupDefFile��5+      $��Browse��68   $
��Left=�6I   $��
712,58,0,0q�6d   $	��Top=�6x   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5,      $��Level Of Confidence��6   $
��Left=�66   $��449,190,0,0q�6Q   $	��Top=�6f   $ -3.74,-2.112&�6~     5-      -   5        $!	 chkListBoxLevelOfConfidence��5-   '   $
��Left=�6L   $	��122��6g   $��419,221,0,0q�6t   $	��Top=�6�   $	��179��6�   6�   �� 5.      -   5        $ txtViewGroup��5.      $
��Left=�60   $	��237��6K   $��
11,106,0,0q�6X   $
 Wrap=�6l   $��TextBox��6�   $	��Top=�6�   $	��395��6�   �� 5/      $��Group Defination��6   $
��Left=�63   $��
148,76,0,0q�6N   $	��Top=�6b   $ -3.74,-2.112&�6z   �� 50      -   5        $ btnClose��50      $��Close��6+   $
��Left=�6;   $��643,312,0,0q�6V   $	��Top=�6k   $	��108��6�   $��31��6�   6�   52      53      � 54   
   -   5        $ progressBarSigTest��54      $
��Left=�62   $��17��6M   $��193,230,0,0q�6Y   $	��Top=�6n   $	��532��6�   55      56      