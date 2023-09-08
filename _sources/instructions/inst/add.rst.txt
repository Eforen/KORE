**************************
ADD (Add Registers)
**************************

Add one register (rs1) to another (rs2) and store the result it in a 3rd destination register (rd)

Note: Arithmetic overflow is ignored.

Formula
:math:`x[rd] \leftarrow x[r_1] + x[r_2]`

.. code-block:: kasm

    ADD rs, r1, r2

..table::
    ============
    Availability
    ============
    RV32I
    ============
    RV64I
    ============

================
Compressed Forms
================
c.add rd, rs2
================
x.mv rd, rs2
================

.. rst-class:: center-align-table-cell

+-----------+---------------------+-------------------+---------+---------+---------------------+---------+
| INST TYPE | 31 - 25             | 24 - 20           | 19 - 15 | 14 - 12 |  11 - 07            | 06 - 00 |
+===========+=====================+===================+=========+=========+=====================+=========+
|  R-TYPE   | 0000000             |        rs2        |   rs1   |  000    |         rd          | 0110011 |
+-----------+---------------------+-------------------+---------+---------+---------------------+---------+