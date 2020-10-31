
*******************
Terminal
*******************

.. tabs::

    .. code-tab:: kpp Kerboscript++

        Terminal.Write "Hello World";

    .. code-tab:: kerboscript Kerboscript

        TERMINAL:WRITE("Hello World").

    .. code-tab:: kvm Kerbo VM

        push "Hello World"
        pop TERMINAL

    .. code-tab:: kasm Kerbo Assembly

        MOV [TERM]+0x08, TMP0
        MOV 0x0048, [TMP0]
        ADD 0x16, TMP0
        MOV 0x0065, [TMP0]
        ADD 0x16, TMP0
        MOV 0x006C, [TMP0]
        ADD 0x16, TMP0
        MOV 0x006C, [TMP0]
        ADD 0x16, TMP0
        MOV 0x006F, [TMP0]
        ADD 0x16, TMP0
        MOV 0x0020, [TMP0]
        ADD 0x16, TMP0
        MOV 0x0057, [TMP0]
        ADD 0x16, TMP0
        MOV 0x006F, [TMP0]
        ADD 0x16, TMP0
        MOV 0x0072, [TMP0]
        ADD 0x16, TMP0
        MOV 0x006C, [TMP0]
        ADD 0x16, TMP0
        MOV 0x0064, [TMP0]