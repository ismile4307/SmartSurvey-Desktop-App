   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	    DBI Scripting3   -DBI_Scripting.Forms.Download.FrmCreateOEExcel   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   -   5          �Name$  frmCreateOEExcel��5   	    � Title$ Create OE Excel��5   	   $��557.708��6!   $��866.136��62    � WindowStartupLocation$ CenterScreen=�6B   .��5      � .+�5   
   �� $��Excel Data Path :��6   $
��Left=�6,   $��	28,54,0,0q�6G   $	��Top=�6Z    J� RenderTransformOrigin$ -0.813,-0.45&�6r   $��96��6�   �� 5   
   -   5         ��Name$ txtExcelDataPath��5      $
��Left=�6,   $��23��6G   $��
129,56,0,0q�6S    �� TextWrapping$
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��575��6�   �� 5	   
   -   5         ��Name$ 	btnBrowse��5	      $��Browse��6$   $
��Left=�65   $��
725,57,0,0q�6P   $	��Top=�6d   $��22��6|   $��75��6�   6�   �� 5
   
   -   5        $ btnRun��5
      $	��Run��6!   $
��Left=�6/   $��
370,87,0,0q�6J   $	��Top=�6^   $	��116��6v   $��33��6�   6�   �� 5   
   -   5        $ btnExit��5      $
��Exit��6"   $
��Left=�61   $��
725,94,0,0q�6L   $	��Top=�6`   $��22��6x   $��75��6�   6�   �� 5   
   $��Project Name :��6   $
��Left=�6)   $��	37,21,0,0q�6D   $	��Top=�6W   $ -3.74,-2.112&�6o   �� 5   
   -   5         ��Name$ comProjectName��5      $
��Left=�6+   $��
130,24,0,0q�6F   $	��Top=�6Z   $	��166��6r   �� 5   
   -   5         ��Name$ chkOnlyTakeOE��5      $��Only Taken OE Sheet��6*   $
��Left=�6H   $��
334,28,0,0q�6c   $	��Top=�6w   �� 5   
   $��Data file Path :��6   $
��Left=�6+   $��
38,171,0,0q�6F   $	��Top=�6Z   $ -3.74,-2.112&�6r   �� 5   
   -   5        $ txtDataFileLocation��5      $
��Left=�6/   $��23��6J   $��129,173,0,0q�6V   $
 Wrap=�6k   $	��Top=�6   $	��575��6�   �� 5   
   -	   5        $ btnBrowseExcelData��5      $��Browse��6-   $
��Left=�6>   $��725,173,0,0q�6Y   $	��Top=�6n   $��75��6�   $ 8.079,-5.321&�6�   $��23��6�   �� 5   
   -
   5        $ 
btnExecute��5      $��Execute��6%   $
��Left=�67   $��488,377,0,0q�6R   $	��Top=�6g   $	��108��6   $��31��6�   $ 0.822,-5.176&�6�   6�   �� 5   
   -   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��488,447,0,0q�6N   $	��Top=�6c   $	��102��6{   $ 8.079,-5.321&�6�   $��31��6�   6�   �� 5   
   -   5        $ chkSelectAll��5      $��
Select All��6)   $
��Left=�6>   $��333,491,0,0q�6Y   $	��Top=�6n   6�   �� 5   
   $��	WrokSheet��6   $
��Left=�6$   $��182,196,0,0q�6?   $	��Top=�6T   $ -3.74,-2.112&�6l   $   Xceed.Wpf.Toolkit.CheckListBox  5   
   -   5        	  Name$	 chkListBoxWorksheet��5      $
��Left=�6<   $	��286��6W   $��132,227,0,0q�6d   $	��Top=�6y   $	��179��6�   6�   �� 5   
   $�� ��6   $
��Left=�6   $��393,277,0,0q�68   $	��Top=�6M   $��78��6e   $	��292��6q   .��5      � .+�5      �� -   5        
 ��Name$
 	lblStatus��5      $��	Status : ��6+   $
��Left=�6?   $��	11,25,0,0q�6Z   $	��Top=�6m   $��25��6�   $	��263��6�   �� 5      -   5        $
 lblProgress��5      $��Progress : ��6-   $
��Left=�6C   $��11,0,0,0q�6^   $	��Top=�6p   $��25��6�   $	��263��6�   5      5      �� 5   
   $��	File Name��6   $
��Left=�6$   $��504,197,0,0q�6?   $	��Top=�6T   $ -3.74,-2.112&�6l   �� 5   
   -   5        $ txtOESyntaxName��5      $
��Left=�6+   $��23��6F   $��392,227,0,0q�6R   $
 Wrap=�6g   $	��Top=�6{   $	��292��6�   $��02.OE_SyntaxV1��6�   $��Center=�6�   $��Center=�6�   �� 5    
   -   5        $ chkDontCreateVar��5       $��Don't Create Variables��6-   $
��Left=�6N   $��470,262,0,0q�6i   $	��Top=�6~   5"      5#      