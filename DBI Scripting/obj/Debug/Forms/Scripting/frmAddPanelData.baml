   M S B A M L   `   `   `  ����  DBI Scripting3    -DBI_Scripting.Forms.Scripting.FrmAddPanelData   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	     � Title$  Insert Panel Data��5   	   $��266.27��6#   $��866.136��63    � WindowStartupLocation$ CenterScreen=�6C   .��5      � $��0,0,0.333,5.667q�6   .+�5   
   �� $��Script Path :��6   $
��Left=�6(   $��	58,49,0,0q�6C   $	��Top=�6V    J� RenderTransformOrigin$ -0.813,-0.45&�6n   $��80��6�   �� 5   
   -   5         ��Name$ txtScriptPath��5      $
��Left=�6)   $��23��6D   $��
143,51,0,0q�6P    �� TextWrapping$
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��575��6�   �� 5	   
   -   5         ��Name$ btnBrowseScript��5	      $��Browse��6*   $
��Left=�6;   $��
738,52,0,0q�6V   $	��Top=�6j   $��22��6�   $��75��6�   6�   �� 5
   
   -   5        $ btnExit��5
      $
��Exit��6"   $
��Left=�61   $��417,194,0,0q�6L   $	��Top=�6a   $��22��6y   $��75��6�   6�   �� 5   
   $��Script Version :��6   $
��Left=�6+   $��
404,16,0,0q�6F   $	��Top=�6Z   �� 5   
   -   5        $ txtScriptVersion��5      $
��Left=�6,   $��23��6G   $��
492,20,0,0q�6S   $
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��226��6�   $��14(�6�   �� 5   
   $��Project Name :��6   $
��Left=�6)   $��	55,15,0,0q�6D   $	��Top=�6W   �� 5   
   -   5        $ txtProjectName��5      $
��Left=�6*   $��23��6E   $��
143,19,0,0q�6Q   $
 Wrap=�6e   $�� ��6y   $	��Top=�6�   $	��226��6�   $��14(�6�   �� 5   
   $��Translated Excel :��6   $
��Left=�6-   $��	37,81,0,0q�6H   $	��Top=�6[   $ -0.813,-0.45&�6s   $	��102��6�   �� 5   
   -   5        $ txtPlaceholderExcel��5      $
��Left=�6/   $��23��6J   $��
143,83,0,0q�6V   $
 Wrap=�6j   $�� ��6~   $	��Top=�6�   $	��575��6�   �� 5   
   -   5        $ btnBrowseExcel��5      $��Browse��6)   $
��Left=�6:   $��
738,84,0,0q�6U   $	��Top=�6i   $��22��6�   $��75��6�   6�   �� 5   
   -   5        $ btnInsertPanelData��5      $��Insert Panel Data��6-   $
��Left=�6I   $��404,114,0,0q�6d   $	��Top=�6y   $	��111��6�   $��33��6�   6�   �� 5   
   -	   5        $ btnKillProcess��5      $��Kill Process��6)   $
��Left=�6@   $��738,150,0,0q�6[   $	��Top=�6p   $��22��6�   $��75��6�   6�   �� 5   
   $	��Top=�6   $ -3.74,-2.112&�6(   $��
70,114,0,0q�6M   $
��Left=�6a   $��	WrokSheet��6|   $  Xceed.Wpf.Toolkit.CheckListBox  5   
   -
   5          Name$ chkListBoxWorksheet��5      $
��Left=�69   $��70��6T   $��143,114,0,0q�6`   $	��Top=�6u   $	��161��6�   6�   � 5   
   -   5         �Name$ progressBar1��5      $
��Left=�6,   $��22��6G   $��319,162,0,0q�6S   $	��Top=�6h   $	��285��6�   5      5      