   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	    DBI Scripting6   0DBI_Scripting.Forms.Analytics.FrmUnPivotOESyntax   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   -   5          �Name$  frmOESyntaxSPSS��5   	    � Title$ FrmUnPivotOESyntax��5   	   $��446.31��6$   $��857.452��64    � WindowStartupLocation$ CenterScreen=�6D    � 
ResizeMode$ NoResize=�6i   .��5      � .+�5   
   �� $��OE Excel file Path :��6   $
��Left=�6/   $��	11,21,0,0q�6J   $	��Top=�6]    J� RenderTransformOrigin$ -3.74,-2.112&�6u   �� 5   
   -   5         ��Name$ txtExcelFileLocation��5      $
��Left=�60   $��23��6K   $��
122,25,0,0q�6W    �� TextWrapping$
 Wrap=�6k   $�� ��6   $	��Top=�6�   $	��614��6�   �� 5	   
   -   5         ��Name$ btnBrowseOEExcel��5	      $��Browse��6+   $
��Left=�6<   $��
749,26,0,0q�6W   $	��Top=�6k   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5
   
   -   5        $ 
btnExecute��5
      $��Execute��6%   $
��Left=�67   $��536,102,0,0q�6R   $	��Top=�6g   $	��108��6   $��31��6�   6�   �� 5   
   -   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��552,149,0,0q�6N   $	��Top=�6c   $��75��6{   $ 8.079,-5.321&�6�   6�   �� 5   
   -   5         ��Name$ chkSelectAll��5      $��
Select All��6)   $
��Left=�6>   $��323,366,0,0q�6Y   $	��Top=�6n   6�   �� 5   
   $��	WrokSheet��6   $
��Left=�6$   $��
186,53,0,0q�6?   $	��Top=�6S   $ -3.74,-2.112&�6k   $   Xceed.Wpf.Toolkit.CheckListBox  5   
   -   5        	  Name$	 chkListBoxWorksheet��5      $
��Left=�6<   $	��303��6W   $��
125,80,0,0q�6d   $	��Top=�6x   $	��186��6�   6�   �� 5   
   -   5        
 ��Name$
 lblNoOfRejectionId��5      $��No of Rejection Id : ��6,   $
��Left=�6L   $��395,314,0,0q�6g   $	��Top=�6|   $ -3.74,-2.112&�6�   �� 5   
   $�� ��6   $
��Left=�6   $��444,194,0,0q�68   $	��Top=�6M   $��78��6e   $	��292��6q   .��5      � .+�5      �� -	   5        $
 	lblStatus��5      $��Status��6+   $
��Left=�6<   $��	10,25,0,0q�6W   $	��Top=�6j   $��25��6�   $	��263��6�   �� 5      -
   5        $
 lblProgress��5      $��Progress : ��6-   $
��Left=�6C   $��10,0,0,0q�6^   $	��Top=�6p   $��25��6�   $	��263��6�   5      5      �� 5   
   $��OE Excel file Path :��6   $
��Left=�6/   $��
323,53,0,0q�6J   $	��Top=�6^   $ -3.74,-2.112&�6v   �� 5   
   -   5        $ txtSaveFileName��5      $
��Left=�6+   $��23��6F   $��
429,56,0,0q�6R   $
 Wrap=�6f   $��	OE_Syntax��6z   $	��Top=�6�   $	��307��6�   5      5      