   M S B A M L   `   `   `  ����X  TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4P:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.ToolkitXceed.Wpf.Toolkit  5   	    DBI Scripting1   +DBI_Scripting.Forms.Download.FrmRemoveMedia   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   NwpfTool:clr-namespace:Xceed.Wpf.Toolkit;assembly=Xceed.Wpf.Toolkit     5   	   4xctk)http://schemas.xceed.com/wpf/xaml/toolkit   5   	   -   5          �Name$  FrmRemoveMedia1��5   @    � Title$ FrmRemoveMedia��5   	   $��500.333��6    $��847.033��61    � WindowStartupLocation$ CenterScreen=�6a   .��5      � .+�5   
   �� $��Download Respondent Id��6   $
��Left=�63   $��	10,10,0,0q�6N   $	��Top=�6a   $	��170��6y   $	��809��6�   .��5	      � .+�5
      �� -   5         ��Name$ btnBrowseDownload��5
      $��Browse��64   $
��Left=�6E   $��
686,49,0,0q�6`   $	��Top=�6t   $��22��6�   $��75��6�   6�   �� 5      -   5        $ btnDownload��5      $��Download��6.   $
��Left=�6A   $��
344,94,0,0q�6\   $	��Top=�6p   $	��111��6�   $��33��6�   6�   �� 5      $��Project Name :��6   $
��Left=�61   $��	219,6,0,0q�6L   $	��Top=�6_   �� 5      -   5         ��Name$ comProjectNameDownload��5      $
��Left=�6;   $��
316,10,0,0q�6V   $	��Top=�6j   $	��165��6�   �� 5      $��Save Location :��6   $
��Left=�62   $��	25,46,0,0q�6M   $	��Top=�6`    J� RenderTransformOrigin$ -0.813,-0.45&�6x   $	��103��6�   �� 5      -   5         ��Name$ txtSaveLocation��5      $
��Left=�63   $��23��6N   $��
128,49,0,0q�6Z    �� TextWrapping$
 Wrap=�6n   $�� ��6�   $	��Top=�6�   $	��536��6�   5      5      �� 5   
   $��Remove Media from Server��6   $
��Left=�65   $��
10,191,0,0q�6P   $	��Top=�6d   $	��227��6|   $	��809��6�   .��5      � $��0,0,-0.333,0.333q�6   .+�5      �� -   5        $ btnBrowseRemove��5      $��Browse��62   $
��Left=�6C   $��
686,49,0,0q�6^   $	��Top=�6r   $��22��6�   $��75��6�   6�   �� 5      -   5        $ 
btnExecute��5      $��Execute��6-   $
��Left=�6?   $��
344,84,0,0q�6Z   $	��Top=�6n   $	��111��6�   $��33��6�   6�   �� 5      $��Project Name :��6   $
��Left=�61   $��	219,6,0,0q�6L   $	��Top=�6_   �� 5      -   5        $ comProjectNameRemove��5      $
��Left=�69   $��
316,10,0,0q�6T   $	��Top=�6h   $	��165��6�   �� 5      $��File Location :��6   $
��Left=�62   $��	25,46,0,0q�6M   $	��Top=�6`   $ -0.813,-0.45&�6x   $	��103��6�   �� 5      -	   5        $ txtExcelFilePath��5      $
��Left=�64   $��23��6O   $��
128,49,0,0q�6[   $
 Wrap=�6o   $�� ��6�   $	��Top=�6�   $	��536��6�   �� 5      -
   5         ��Name$ lblDownloadStatus��5      $��Now Remove :��63   $
��Left=�6J   $��279,142,0,0q�6e   $	��Top=�6z   $ -0.813,-0.45&�6�   $	��157��6�   �� 5      -   5        $ lblDownloadStatusMessage��5      $��Server Message :��6:   $
��Left=�6U   $��267,168,-0.333,0q�6p   $	��Top=�6�   $ -0.813,-0.45&�6�   $	��530��6�   �� 5      $	��Top=�6   $ -3.74,-2.112&�60   $��
126,79,0,0q�6U   $
��Left=�6i   $��	WrokSheet��6�   $   Xceed.Wpf.Toolkit.CheckListBox  5      -   5        	  Name$	 chkListBoxWorksheet��5   '   $
��Left=�6D   $��90��6_   $��76,106,0,-85q�6k   $	��Top=�6�   $	��179��6�   6�   5      5      �� 5    
   -   5        $ btnClose��5       $��Close��6#   $
��Left=�63   $��361,423,0,0q�6N   $	��Top=�6c   $	��111��6{   $��33��6�   6�   5$      5%      