   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	   i eMicrosoft.TeamFoundation.Controls, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls%Microsoft.TeamFoundation.Controls.WPF 5   	    DBI Scripting/   )DBI_Scripting.Forms.Download.FrmUpdateLOI   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   nWPF^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls     5   	   -   5          �Name$  frmDownloadLOI��5   t    � Title$ 
Update LOI��5   	   $��486.252��6   $��768.885��6-    � WindowStartupLocation$ CenterScreen=�6=    � 
ResizeMode$ NoResize=�6b    � Icon$3 -/DBI Scripting;component/Images/reject_64.png��6x   .��5      � .+�5   
   �� $��Data file Path :��6   $
��Left=�6+   $��	11,59,0,0q�6F   $	��Top=�6Y    J� RenderTransformOrigin$ -3.74,-2.112&�6q   �� 5   
   -   5         ��Name$ txtDataFileLocation��5      $
��Left=�6/   $��23��6J   $��
103,62,0,0q�6V    �� TextWrapping$
 Wrap=�6j   $	��Top=�6~   $	��551��6�   �� 5	   
   -   5         ��Name$ btnBrowseDataFile��5	      $��Browse��6,   $
��Left=�6=   $��
659,62,0,0q�6X   $	��Top=�6l   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5
   
   -   5        $ 	btnReject��5
      $��Reject��6$   $
��Left=�65   $��459,134,0,0q�6P   $	��Top=�6e   $	��108��6}   $��31��6�   $y�Hidden=�6�   �� 5   
   -   5        $ 	btnDelete��5      $��Delete��6$   $
��Left=�65   $��594,134,0,0q�6P   $	��Top=�6e   $	��108��6}   $��31��6�   $y�Hidden=�6�   �� 5   
   -   5        $ 	btnUpdate��5      $��
Update LOI��6$   $
��Left=�69   $��459,134,0,0q�6T   $	��Top=�6i   $	��108��6�   $��31��6�   6�   �� 5   
   -   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��532,343,0,0q�6N   $	��Top=�6c   $	��102��6{   $ 8.079,-5.321&�6�   $��31��6�   6�   �� 5   
   $��Project Name :��6   $
��Left=�6)   $��	10,26,0,0q�6D   $	��Top=�6W   $ -3.74,-2.112&�6o   �� 5   
   -   5        	 ��Name$	 comProjectName��5      $
��Left=�6+   $��
103,29,0,0q�6F   $	��Top=�6Z   $	��166��6r   �� 5   
   -	   5        
 ��Name$
 chkSelectAll��5      $��
Select All��6)   $
��Left=�6>   $��442,422,0,0q�6Y   $	��Top=�6n   6�   �� 5   
   $��	WrokSheet��6   $
��Left=�6$   $��
66,107,0,0q�6?   $	��Top=�6S   $ -3.74,-2.112&�6k   �� 5   
   $��Respondent Id��6   $
��Left=�6(   $��272,107,0,0q�6C   $	��Top=�6X   $ -3.74,-2.112&�6p   $   Xceed.Wpf.Toolkit.CheckListBox  5   
   -
   5          Name$ chkListBoxWorksheet��5      $
��Left=�6<   $	��182��6W   $��
16,134,0,0q�6d   $	��Top=�6x   $	��179��6�   6�     5   
   -   5        $ chkListBoxRespondentId��5      $
��Left=�6?   $	��303��6Z   $��216,134,0,0q�6g   $	��Top=�6|   $	��205��6�   6�   �� 5   
   -   5         ��Name$ lblNoOfRejectionId��5      $��No of Updated Id : ��6,   $
��Left=�6J   $��
16,338,0,0q�6e   $	��Top=�6y   $ -3.74,-2.112&�6�   �� 5   
   $�� ��6   $
��Left=�6   $��442,238,0,0q�68   $	��Top=�6M   $��78��6e   $	��292��6q   .��5      � .+�5      �� -   5        $ 	lblStatus��5      $��Status��6+   $
��Left=�6<   $��	10,25,0,0q�6W   $	��Top=�6j   $��25��6�   $	��263��6�   �� 5      -   5        $ lblProgress��5      $��Progress : ��6-   $
��Left=�6C   $��10,0,0,0q�6^   $	��Top=�6p   $��25��6�   $	��263��6�   5      5      �� 5   
   -   5        $ btnUpdate_Copy��5      $��Update��6)   $
��Left=�6:   $��594,134,0,0q�6U   $	��Top=�6j   $	��108��6�   $��31��6�   $y�Hidden=�6�   �� 5   
   -   5        $ btnUpdate_Copy1��5      $��Update��6*   $
��Left=�6;   $��459,188,0,0q�6V   $	��Top=�6k   $	��108��6�   $��31��6�   $y�Hidden=�6�   �� 5   
   -   5        $ btnUpdate_Copy2��5      $��Update��6*   $
��Left=�6;   $��594,188,0,0q�6V   $	��Top=�6k   $	��108��6�   $��31��6�   $y�Hidden=�6�   �� 5   
   -   5        $ btnUpdate_FIFS��5      $��Update FIFS��6)   $
��Left=�6?   $��594,134,0,0q�6Z   $	��Top=�6o   $	��108��6�   $��31��6�   6�   �� 5    
   -   5        $ btnUpdateTemplete��5       $��Templete��6,   $
��Left=�6?   $��
44,385,0,0q�6Z   $	��Top=�6n   $	��108��6�   $��31��6�   6�   �� 5!   
   -   5        $ btnUpdate_MobileNo��5!      $��Update MobileNo��6-   $
��Left=�6G   $��459,188,0,0q�6b   $	��Top=�6w   $	��108��6�   $��31��6�   6�   5#      5$      