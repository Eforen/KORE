.text
    rdcycle t0
    rdtime a0
    rdtimeh a1
    csrwi cycle, 0
    csrrs t2, cycle, x0
