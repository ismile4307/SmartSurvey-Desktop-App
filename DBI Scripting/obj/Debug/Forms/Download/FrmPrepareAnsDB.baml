   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	   i eMicrosoft.TeamFoundation.Controls, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a�^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls%Microsoft.TeamFoundation.Controls.WPF 5   	    DBI Scripting2   ,DBI_Scripting.Forms.Download.FrmPrepareAnsDB   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   nWPF^clr-namespace:Microsoft.TeamFoundation.Controls.WPF;assembly=Microsoft.TeamFoundation.Controls     5   	   -   5          �Name$  FrmPrepareAnsDB1��5   	    � Title$  Prepare Ans DB from Server��5   	   $��520.252��6,   $��768.885��6=    � WindowStartupLocation$ CenterScreen=�6M   .��5      � -   5        .+�5   
   �� $��Data file Path :��6   $
��Left=�6+   $��	13,59,0,0q�6F   $	��Top=�6Y    J� RenderTransformOrigin$ -3.74,-2.112&�6q   �� 5	   
   -   5         ��Name$ txtDataFileLocation��5	      $
��Left=�6/   $��23��6J   $��
103,62,0,0q�6V    �� TextWrapping$
 Wrap=�6j   $	��Top=�6~   $	��551��6�   �� 5
   
   -   5         ��Name$ btnBrowseDataFile��5
      $��Browse��6,   $
��Left=�6=   $��
659,62,0,0q�6X   $	��Top=�6l   $��75��6�   $ 8.079,-5.321&�6�   6�   �� 5   
   -   5        $ 	btnReject��5      $��Reject��6$   $
��Left=�65   $��459,170,0,0q�6P   $	��Top=�6e   $	��108��6}   $��31��6�   6�   �� 5   
   -   5        $ 	btnDelete��5      $��Delete��6$   $
��Left=�65   $��594,170,0,0q�6P   $	��Top=�6e   $	��108��6}   $��31��6�   6�   �� 5   
   -   5        $ btnRedoReject��5      $��Redo Reject��6(   $
��Left=�6>   $��459,223,0,0q�6Y   $	��Top=�6n   $	��108��6�   $��31��6�   6�   �� 5   
   -   5        $ btnTestToLive��5      $��Test to Live��6(   $
��Left=�6?   $��594,223,0,0q�6Z   $	��Top=�6o   $	��108��6�   $��31��6�   6�   �� 5   
   -	   5        $ btnClose��5      $��Close��6#   $
��Left=�63   $��532,379,0,0q�6N   $	��Top=�6c   $	��102��6{   $ 8.079,-5.321&�6�   $��31��6�   �� 5   
   $��Project Name :��6   $
��Left=�6)   $��	10,26,0,0q�6D   $	��Top=�6W   $ -3.74,-2.112&�6o   �� 5   
   -
   5         ��Name$ comProjectName��5      $
��Left=�6+   $��
103,29,0,0q�6F   $	��Top=�6Z   $	��166��6r   �� 5   
   -   5         ��Name$ chkSelectAll��5      $��
Select All��6)   $
��Left=�6>   $��442,458,0,0q�6Y   $	��Top=�6n   6�   �� 5   
   $��	WrokSheet��6   $
��Left=�6$   $��
66,143,0,0q�6?   $	��Top=�6S   $ -3.74,-2.112&�6k   �� 5   
   $��Respondent Id��6   $
��Left=�6(   $��272,143,0,0q�6C   $	��Top=�6X   $ -3.74,-2.112&�6p   $   Xceed.Wpf.Toolkit.CheckListBox  5   
   -   5        	  Name$	 chkListBoxWorksheet��5      $
��Left=�6<   $	��182��6W   $��
16,170,0,0q�6d   $	��Top=�6x   $	��179��6�   6�     5   
   -   5        $	 chkListBoxRespondentId��5      $
��Left=�6?   $	��303��6Z   $��216,170,0,0q�6g   $	��Top=�6|   $	��205��6�   6�   �� 5   
   -   5        
 ��Name$
 lblNoOfRejectionId��5      $��No of Rejection Id : ��6,   $
��Left=�6L   $��
16,374,0,0q�6g   $	��Top=�6{   $ -3.74,-2.112&�6�   �� 5   
   $�� ��6   $
��Left=�6   $��442,274,0,0q�68   $	��Top=�6M   $��78��6e   $	��292��6q   .��5      � .+�5      �� -   5        $
 	lblStatus��5      $��Status��6+   $
��Left=�6<   $��10,61,0,-31.333q�6W   $	��Top=�6p   $��25��6�   $	��263��6�   �� 5      -   5        $
 lblProgress��5      $��Progress : ��6-   $
��Left=�6C   $��10,36,0,-6.333q�6^   $	��Top=�6v   $��25��6�   $	��263��6�   5      5      �� 5   
   $��Ans DB Path :��6   $
��Left=�6(   $��	18,92,0,0q�6C   $	��Top=�6V   $ -0.813,-0.45&�6n   $��80��6�   �� 5   
   -   5        $ txtAnsDBPath��5      $
��Left=�6(   $��23��6C   $��
103,94,0,0q�6O   $
 Wrap=�6c   $�� ��6w   $	��Top=�6   $	��551��6�   �� 5    
   -   5        $ btnBrowseAnsDB��5       $��Browse��6)   $
��Left=�6:   $��
660,95,0,0q�6U   $	��Top=�6i   $��22��6�   $��75��6�   6�   5(      5)      