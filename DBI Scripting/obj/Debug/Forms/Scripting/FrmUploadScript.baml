   M S B A M L   `   `   `  ����  DBI Scripting3    -DBI_Scripting.Forms.Scripting.FrmUploadScript   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   -   5          �Name$  frmUploadScript��5   @    � Title$ Upload Script��5   	   $��271.265��6   $��866.136��60    � WindowStartupLocation$ CenterScreen=�6@   .��5      � .+�5   
   �� $��Script Path :��6   $
��Left=�6(   $��	45,83,0,0q�6C   $	��Top=�6V    J� RenderTransformOrigin$ -0.813,-0.45&�6n   $��80��6�   �� 5   
   -   5         ��Name$ txtScriptPath��5      $
��Left=�6)   $��23��6D   $��
130,85,0,0q�6P    �� TextWrapping$
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��575��6�   �� 5	   
   -   5         ��Name$ 	btnBrowse��5	      $��Browse��6$   $
��Left=�65   $��
725,86,0,0q�6P   $	��Top=�6d   $��22��6|   $��75��6�   6�   �� 5
   
   -   5        $ 	btnUpload��5
      $��Upload��6$   $
��Left=�65   $��370,126,0,0q�6P   $	��Top=�6e   $	��111��6}   $��33��6�   6�   �� 5   
   -   5        $ btnExit��5      $
��Exit��6"   $
��Left=�61   $��389,178,0,0q�6L   $	��Top=�6a   $��22��6y   $��75��6�   6�   �� 5   
   $��Script Version :��6   $
��Left=�6+   $��	42,47,0,0q�6F   $	��Top=�6Y   �� 5   
   -   5        $ txtScriptVersion��5      $
��Left=�6,   $��23��6G   $��
130,51,0,0q�6S   $
 Wrap=�6g   $�� ��6{   $	��Top=�6�   $	��226��6�   $��14(�6�   �� 5   
   $��Project Name :��6   $
��Left=�6)   $��	42,13,0,0q�6D   $	��Top=�6W   �� 5   
   -   5        $ txtProjectName��5      $
��Left=�6*   $��23��6E   $��
130,17,0,0q�6Q   $
 Wrap=�6e   $�� ��6y   $	��Top=�6�   $	��226��6�   $��14(�6�   �� 5   
   -   5        $ btnUpload_Copy��5      $��Upload��6)   $
��Left=�6:   $��555,126,0,0q�6U   $	��Top=�6j   $	��111��6�   $��33��6�   6�   $  Xceed.Wpf.Toolkit.CheckListBox  5   
   -	   5          Name$ chkListBoxQFiles��5      $
��Left=�66   $��89��6Q   $��130,126,0,0q�6]   $	��Top=�6r   $	��186��6�   6�   �� 5   
   $��	Q Files :��6   $
��Left=�6$   $��
68,126,0,0q�6?   $	��Top=�6S   $ -0.813,-0.45&�6k   $��55��6�   5      5      