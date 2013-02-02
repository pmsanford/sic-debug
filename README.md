SIC Debug
=========

This is an interpreter and basic debugger for the Simple Instructional Machine as laid out in Leland L. Beck's book. The initial commit is how I left it when I wrote it in my junior year of college. I will be making periodic improvements and cleaning up the code.

It is designed to run both Simple SIC and SIC/XE programs in the standard format and the extended relocatable format. I will be first ironing out bugs in the current functionality and then refactoring it for usability and reliability.

Every SIC opcode supported should work the same as it does in SICSIM, and as specified by the book. If not, let me know.

Exhaustive list of supported SIC/XE opcodes:

ADD, SUB, J, JEQ, JGT, JLT, MUL, DIV, STA, LDA, COMP, TIX, LDB, STB, STL, LDL, RSUB, JSUB, LDS, LDT, STS, STT, ADDR, CLEAR, COMPR, DIVR, RMO, SUBR, TIXR, LDX, STX, LDCH, STCH, SHIFTL, SHIFTR, RD, TD, WD, AND, OR

Description of features:

* In the files menu, you can load files as I/O devices. The number of the I/O device is the argument to the SIC device instructions. There is also a checkbox for allowing or disallowing writing to files. If you disallow writing, it will turn WD instructions into NOOPs (do-nothing instructions). If one is encountered while executing a program with writing disabled, a message to that effect will be printed.
* You can also clear memory. This will write 0xFF to every memory location (Beck specifies that memory is initialized to 0xFF in his book).
* Start and End Addr refer to the beginning and ending addresses of the memory you wish to view. "View" will output memory values between those two limits. These values (and all values on the form) are in hex.
* Load will load a program into memory (note that this does not use the 'loader' program provided with sicsim; I load it into memory myself. The loader program will work, though. See below.)
* Run will begin executing instructions from memory starting at the address (in hex, as all values on the form) in the 'Entry Pt' box. Step will execute a single instruction. Both of these functions will cause a memory and register dump to be output based on the start and end addr.
* The Add Bkpt button will add a breakpoint at the address specified in the box above (in hex, as all values on the form). It will be added to the list of breakpoints below. Any time the program counter is set to that address, it will halt execution before executing the instruction and output a memory dump.
* The instruction list on the right shows the last instructions executed. If the 'Full Trace' box is unchecked, it will show the last 15 instructions. If it is checked, it will show every instruction. Because of the way this is implemented currently, however, that is very slow.

The loader used in sicsim is tricky, and you have to do some hand-editing of the object code to get it to work (in sicsim as well, I'll explain below). 

I'll reproduce the relevant part of the .lst file here (lines omitted for brevity replaced by ...):

>020- 00014 6B2062                       LDB       ADDR          . MOVE IT TO BASE REG B
>...
>027- 00029 4B2008            LOOP       JSUB      GETPAIR       . GET A BYTE OF SOURCE
>028- 0002C                              BASE      ADDR
>029- 0002C 57A04A                       STCH      ADDR,X        . STORE AT B+X WITH 0 DISP
>030- 0002F                              NOBASE   
>031- 0002F B810                         TIXR      X             . (X)=(X)+1
>032- 00031 3F2FF5                       J         LOOP
>...
>065- 00079                   ADDR       RESB      1             . STORAGE 1ST FOR LOAD POINT
>066- 0007A                   ADDR2      RESB      1             .    THEN FOR START ADDRESS
>067- 0007B                   ADDR3      RESB      1


The goal here is to appropriate the base-relative addressing functionality of SIC/XE and use it in a different way. He's setting the BASE to ADDR (ADDR is read in from the object file, it's the address the program wants to be placed at (usually the number after START)). He then grabs a byte of the object file on line 27, turns on base addressing on 28, stores a character indexing off the address at 29, and turns base back off on 30. However, you'll notice that the ADDR label is only 0x4A bytes away from the stch instruction. That's why the STCH instruction gets assembled as 57A04A. A is 1010, x is 1 and p is 1, for indexed PC-relative. This is obviously not where we want to start putting our program into memory: It just starts overwriting the storage the loader allocated! What must be done afterwards is to change 57A04A to 57C000. C is 1100, indexed base-relative. We also want to start right at the address of our base, so the displacement is set to 0. I assume it was written this way for efficiency's sake. (Your first instinct might be to just put an @ in front of the ADDR,X, but indexing is not allowed on indirect addresses). There is, of course, a way to do it without fiddling with the object code afterwords, using indirect addressing, but it requires doing an address calculation for each byte of the source file.