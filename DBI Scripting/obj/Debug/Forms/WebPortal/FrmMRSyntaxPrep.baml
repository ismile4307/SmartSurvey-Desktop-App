   M S B A M L   `   `   `  ����  DBI Scripting3    -DBI_Scripting.Forms.WebPortal.FrmMRSyntaxPrep   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      -   5          �Name$  frmAnalysisTable��5   <    � Title$ MR Syntax Preparation��5      $��378.148��6$   $��761.071��65    � WindowStartupLocation$ CenterScreen=�6E   .��5      � .+�5   
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
   $��SPSS Data File :��6   $
��Left=�6+   $��	16,44,0,0q�6F   $	��Top=�6Y   $ -3.74,-2.112&�6q   �� 5   
   -   5        $ txtSPSSPath��5      $
��Left=�6'   $��23��6B   $��
106,47,0,0q�6N   $
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��551��6�   �� 5   
   -   5        $ btnBrowseSPSSData��5      $��Browse��6,   $
��Left=�6=   $��
662,47,0,0q�6X   $	��Top=�6l   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   $��Project Code :��6   $
��Left=�6)   $��	22,77,0,0q�6D   $	��Top=�6W   $ -3.74,-2.112&�6o   �� 5   
   -   5        $ txtProjectCode��5      $
��Left=�6*   $��23��6E   $��
106,80,0,0q�6Q   $
 Wrap=�6e   $�� ��6y   $	��Top=�6�   $	��181��6�   �� 5   
   -   5        $ btnGetStructureHelp��5      $��Get Structure Help��6.   $
��Left=�6K   $��
537,80,0,0q�6f   $	��Top=�6z   $	��120��6�   $ 8.079,-5.321&�6�   �� 5   
   $	��Top=�6   $ -3.74,-2.112&�6(   $��158,117,0,0q�6M   $
��Left=�6b   $��	WrokSheet��6}   $  Xceed.Wpf.Toolkit.CheckListBox  5   
   -   5          Name$ chkListBoxWorksheet��5      $
��Left=�69   $	��182��6T   $��108,144,0,0q�6a   $	��Top=�6v   $	��179��6�   6�   �� 5   
   -	   5        $ btnRun��5      $	��Run��6!   $
��Left=�6/   $��537,184,0,0q�6J   $	��Top=�6_   $	��108��6w   $��31��6�   6�   �� 5   
   -
   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��537,229,0,0q�6N   $	��Top=�6c   $	��108��6{   $��31��6�   6�   � 5   
   -   5         �Name$ progressBar1��5      $
��Left=�6,   $��20��6G   $��
305,80,0,0q�6S   $	��Top=�6g   $	��213��6   �� 5   
   $	��Top=�6   $ -3.74,-2.112&�6(   $��355,117,0,0q�6M   $
��Left=�6b   $��MR Variables��6}     5   
   -   5        $ chkListBoxMRVarList��5      $
��Left=�69   $	��182��6T   $��305,144,0,0q�6a   $	��Top=�6v   $	��179��6�   6�   �� 5   
   -   5        $ 	btnAddAll��5      $��Add All��6$   $
��Left=�66   $��537,295,0,0q�6Q   $	��Top=�6f   $	��108��6~   $��31��6�   �� 5   
   -   5        	 ��Name$	 chkSelectAll��5      $��
Select All��6)   $
��Left=�6>   $��493,270,0,0q�6Y   $	��Top=�6n   6�   5      5      