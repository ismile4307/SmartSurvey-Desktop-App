   M S B A M L   `   `   `  ����  DBI Scripting4    .DBI_Scripting.Forms.Analytics.FrmAnalysisTable   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   -   5          �Name$  frmAnalysisTable��5   @    � Title$ Analysis Table Syntax��5   	   $��378.148��6'   $��761.071��68    � WindowStartupLocation$ CenterScreen=�6H   .��5      � .+�5   
   �� $��Structure Excel :��6   $
��Left=�6,   $��	14,12,0,0q�6G   $	��Top=�6Z    J� RenderTransformOrigin$ -3.74,-2.112&�6r   �� 5   
   -   5         ��Name$ txtAnalysisExcelPath��5      $
��Left=�60   $��23��6K   $��
106,15,0,0q�6W    �� TextWrapping$
 Wrap=�6k   $�� ��6   $	��Top=�6�   $	��551��6�   �� 5	   
   -   5         ��Name$ btnBrowseStructureExcel��5	      $��Browse��62   $
��Left=�6C   $��
662,15,0,0q�6^   $	��Top=�6r   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5
   
   $��Banner :��6   $
��Left=�6#   $��	54,44,0,0q�6>   $	��Top=�6Q   $ -3.74,-2.112&�6i   �� 5   
   -   5        $ txtBannerPath��5      $
��Left=�6)   $��23��6D   $��
106,47,0,0q�6P   $
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��551��6�   �� 5   
   -   5        $ btnBrowseBanner��5      $��Browse��6*   $
��Left=�6;   $��
662,47,0,0q�6V   $	��Top=�6j   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   $��File Name :��6   $
��Left=�6&   $��	37,77,0,0q�6A   $	��Top=�6T   $ -3.74,-2.112&�6l   �� 5   
   -   5        $ txtFileName��5      $
��Left=�6'   $��23��6B   $��
106,80,0,0q�6N   $
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��354��6�   �� 5   
   -   5        $ btnGetStructureHelp��5      $��Get Structure Help��6.   $
��Left=�6K   $��207,291,0,0q�6f   $	��Top=�6{   $	��120��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   -   5        $ btnGetBannerHelp��5      $��Get Banner Help��6+   $
��Left=�6E   $��345,291,0,0q�6`   $	��Top=�6u   $	��115��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   $	��Top=�6   $ -3.74,-2.112&�6(   $��
64,117,0,0q�6M   $
��Left=�6a   $��	WrokSheet��6|   $  Xceed.Wpf.Toolkit.CheckListBox  5   
   -	   5          Name$ chkListBoxWorksheet��5      $
��Left=�69   $	��182��6T   $��
14,144,0,0q�6a   $	��Top=�6u   $	��179��6�   6�   �� 5   
   $�� ��6   $
��Left=�6   $��198,168,0,0q�68   $	��Top=�6M   $	��109��6e   $	��262��6r   .��5      � .+�5      	� -
   5         	�Name$ radiobtnRecode��5      $��Recode Syntax��66   $
��Left=�6N   $��	22,10,0,0q�6i   $	��Top=�6|   	� 5      -   5        $ radiobtnTableCpt��5      $��Table Cpt Syntax��68   $
��Left=�6S   $��	22,36,0,0q�6n   $	��Top=�6�   	� 5      -   5        $ radiobtnTableCount��5      $��Table Count Syntax��6:   $
��Left=�6W   $��	22,63,0,0q�6r   $	��Top=�6�   �� 5      -   5        $ txtDecimalPlace��5      $��Center=�63   $��23��6P   $��158,33,27,29.333q�6\   $
 Wrap=�6v   $��1��6�   $��Center=�6�   $��65��6�   5      5      �� 5   
   -   5        $ btnRun��5      $	��Run��6!   $
��Left=�6/   $��538,144,0,0q�6J   $	��Top=�6_   $	��108��6w   $��31��6�   6�   �� 5   
   -   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��538,226,0,0q�6N   $	��Top=�6c   $	��108��6{   $��31��6�   6�   �� 5   
   -   5        	 ��Name$	 comBaseType��5      $
��Left=�6(   $��290,142,0,0q�6C   $	��Top=�6X   $	��170��6p   �� 5   
   $	��Top=�6   $ -3.74,-2.112&�6(   $��207,139,0,0q�6M   $
��Left=�6b   $��Base Type :��6}   �� 5    
   -   5        $	 comAnalysisType��5       $
��Left=�6,   $��290,111,0,0q�6G   $	��Top=�6\   $	��170��6t   �� 5!   
   $	��Top=�6   $ -3.74,-2.112&�6(   $��188,108,0,0q�6M   $
��Left=�6b   $��Analysis Type :��6}   5#      5$      