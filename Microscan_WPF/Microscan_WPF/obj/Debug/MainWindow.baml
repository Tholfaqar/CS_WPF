   M S B A M L   `   `   `  ����H  DMicroscan_WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Microscan_WPFMicroscan_WPF  5   	       Microscan_WPF.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:Microscan_WPF     5   	   -   5          �Name$  Microscan_App��5   	    � Title$ Microscan app��5   	   $	��800��6   $
��1080��6,    � 
ResizeMode$ NoResize=�6w   .��5	      � -   5         �Name$ MicroscanApp��5	      $��0,0,0,0q�6!   $
��1080��62   $	��Top=�6@    � ColumnDefinitions 5
   
   �� 5      $��0* �6   �� 5      $	��17* �6   �� 5      $
��125* �6   �� 5      $	��13* �6   �� 5      $	��52* �6   �� 5      $
��934* �6   .+�5   
   7  1System.Windows.Forms.Integration.WindowsFormsHost  -   5          Name$ WFHost1��5      $��5��6,   $
��Left=�6<   $��0,0,200,400q�6W   $	��700��6l   �� 5   
   -   5         ��Name$ RunMode_btn��5      $��Run Mode��6&   $
��Left=�69   $��	32,58,0,0q�6T   $	��Top=�6g   $��75��6   $��2��6�   6�   �� 5   
   -   5        $ Trigger_btn��5      $��Trigger��6&   $
��Left=�68   $��
32,118,0,0q�6S   $	��Top=�6g   $��75��6   $��2��6�   6�   �� 5   
   -   5        $ SetupMode_btn��5      $��
Setup Mode��6(   $
��Left=�6=   $��
32,178,0,0q�6X   $	��Top=�6l   $��75��6�   $��2��6�   6�   �� 5   
   -   5        $ LoadAVP_btn��5      $��Load AVP��6&   $
��Left=�69   $��
32,240,0,0q�6T   $	��Top=�6h   $��75��6�   $��2��6�   6�   �� 5   
   -   5        $ CreateAVP_btn��5      $��
Create AVP��6(   $
��Left=�6=   $��
32,305,0,0q�6X   $	��Top=�6l   $��75��6�   $��2��6�   6�   �� 5   
   -	   5         ��Name$ textBox1��5      $��5��6$   $
��Left=�64   $��23��6O   $��
730,43,0,0q�6[    �� TextWrapping$
 Wrap=�6o   $	��Top=�6�   $	��120��6�   �� 5   
   -
   5        $ textBox2��5      $��5��6$   $
��Left=�64   $��23��6O   $��730,124,0,0q�6[   $
 Wrap=�6p   $	��Top=�6�   $	��120��6�   5      5      