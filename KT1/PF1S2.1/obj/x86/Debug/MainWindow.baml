   M S B A M L   `   `   `  ����R  NSharpGL.WPF, Version=2.3.0.1, Culture=neutral, PublicKeyToken=27fc851303210b27>.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPFSharpGL.WPF  5   	    PF1S2.1    Transformations.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   BsharpGL.clr-namespace:SharpGL.WPF;assembly=SharpGL.WPF     5   	   -   5          � Title$  SharpGL WPF Application��5   	   $	��600��6)   $	��800��66   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��200* �6   �� 5	      $
��253* �6   �� 5
      $
��339* �6   .+�5   
      SharpGL.WPF.OpenGLControl  -   5        $��openGLControl��5         DrawFPS .    RenderContextType$	 FBO=�5         	FrameRate$ 20Q�64   $��0,37,0,0q�6C   $��3��6U   �� 5   
   -   5        $
��Left=�5      $��	53,10,0,0q�6O   $	��Top=�6b   $��22��6z   $	��192��6�   $��1��6�    � Minimum$ 0Q�6�    � Maximum$	 360Q�6�   �� 5   
   $��Rotiraj za:��6   $
��Left=�6&   $��	189,5,0,0q�6A   $	��Top=�6T   $��32��6l   $��64��6x   $��2��6�   �� 5   
   -   5        $
��Left=�5      $��	175,5,0,0q�6.   $	��Top=�6A   $	��136��6Y   $��2��6e   $��26��6u   .@�5      �� .��6   Crvena6#   �� 5      .��6   Plava6"   �� 5      .��6   Bela6!   5      �� 5   
   $��Ambijentalna boja:��6   $
��Left=�6-   $��40,5,0,0q�6H   $	��Top=�6Z   $	��135��6r   $��2��6~   �� 5   
   -   5        $��Pomeri��5      $��22��6"   $	��Top=�6.   $��
92,10,36,0q�6F   6Z   �� 5   
   -   5        $��	inputText��5      $
��Left=�6#   $��23��6>   $��	20,10,0,0q�6J    �� TextWrapping$
 Wrap=�6]   $	��Top=�6q   $��72��6�   �� 5   
   
���   �$��1q�6%   $
��Left=�69   $��23��6T   $��	20,10,0,0q�6`   $	��Top=�6s   $	��144��6�   5      5       