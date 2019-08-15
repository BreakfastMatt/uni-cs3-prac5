  // Do learn to insert your names and a brief description of what the program is supposed to do!

  // This is a skeleton program for developing a parser for Modula-2 declarations
  // Matthew Lewis, Liam Searle, Makungu Chansa
  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;
    }

  } // Token

  class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output;

    static string NewFileName(string oldFileName, string ext) {
    // Creates new file name by changing extension of oldFileName to ext
      int i = oldFileName.LastIndexOf('.');
      if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
    // Displays errorMessage on standard output and on reflected output
      Console.WriteLine(errorMessage);
      output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
    // Abandons parsing after issuing error message
      ReportError(errorMessage);
      output.Close();
      System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int 
      noSym        =  0,
      EOFSym       =  1,
      identSym     =  2,
      numSym       =  3, 
      lparenSym    =  4, 
      rparenSym    =  5, 
      OFSym        =  6, //"OF"
      TOSym        =  7, //"TO"
      typeSym      =  8, //"TYPE"
      varSym       =  9, //"VAR"
      equalSym     =  10, 
      lsquareSym   =  11,
      rsquareSym   =  12,
      arraySym     =  13, //"ARRAY"
      recordSym    =  14, //"RECORD"
      setSym       =  15, //"SET"
      singleDotSym =  16, //"."
      doubleDotSym =  17, //".."
      commaSym     =  18,
      pointerSym   =  19, //"POINTER"
      semiColonSym =  20, 
      colonSym     =  21,
      lcurveSym    =  22,
      rcurveSym    =  23,
      endSym       =  24;  //"END"


    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
    // Obtains next character ch from input, or CHR(0) if EOF reached
    // Reflect ch to output
      if (atEndOfFile) ch = EOF;
      else {
        ch = input.ReadChar();
        atEndOfFile = (ch == EOF);
        if (!atEndOfFile) output.Write(ch);
      }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym;

    static void GetSym() { //we changed stuff here.
    // Scans for next sym from input
      StringBuilder symLex = new StringBuilder();
      int symKind = noSym;
      while (ch > EOF && ch <= ' ') GetChar();
      int symKind = noSym;

        if (Char.IsLetter(ch))
        {
            do
            {
                symLex.Append(ch); GetChar();
            } while (Char.IsLetterOrDigit(ch) || ch == '.'); // need to change this.
            //checks if special thingymobob
            switch (symLex)
            {
                case "OF":
                    symKind = OFSym;  GetChar();
                    break;
                case "TO":
                    symKind = TOSym;  GetChar();
                    break;
                case "TYPE":
                    symKind = typeSym; GetChar();
                    break;
                case "VAR":
                    symKind = varSym; GetChar();
                    break;
                case "ARRAY":
                    symKind = arraySym; GetChar();
                    break;
                case "RECORD":
                    symKind = recordSym; GetChar();
                    break;
                case "SET":
                    symKind = setSym; GetChar();
                    break;
                case ".":
                    symKind = singleDotSym; GetChar();
                    break;
                case "..":
                    symKind = doubleDotSym; GetChar();
                    break;
                case "POINTER":
                    symKind = pointerSym; GetChar();
                    break;
                case "END":
                    symKind = endSym; GetChar();
                    break;
                default: symKind = identSym;
            }
            symKind = identSym;
        }
        else if (Char.IsDigit(ch))
        {
            do
            {
                symLex.Append(ch); GetChar();
            } while (Char.IsDigit(ch));
            symKind = numSym;
        }
        else
        {
            symLex.Append(ch);
            switch (ch)
            {
                case EOF:
                    symLex = new StringBuilder("EOF"); // special case
                    break; // no need to GetChar
                case '=':
                    symKind = equalSym; GetChar();
                    break;
                case '(':
                    symKind = lparenSym; GetChar();
                    break;
                case ')':
                    symKind = rparenSym; GetChar();
                    break;
                case '[':
                    symKind = lsquareSym; GetChar();
                    break;
                case ']':
                    symKind = rsquareSym; GetChar();
                    break;
                case '{':
                    symKind = lcurveSym; GetChar();
                    break;
                case '}':
                    symKind = rcurveSym; GetChar();
                    break;
                case ':':
                    symKind = colonSym; GetChar();
                    break;
                case ';':
                    symKind = semiColonSym; GetChar();
                    break;
                case ',':
                    symKind = commaSym; GetChar();
                    break;
                default:
                    symKind = noSym; GetChar();
                    break;
            }
        }

        sym = new Token(symKind, symLex.ToString());
    } // GetSym

  /*  ++++ Commented out for the moment

    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++

    static void Accept(int wantedSym, string errorMessage) {
    // Checks that lookahead token is wantedSym
      if (sym.kind == wantedSym) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Accept(IntSet allowedSet, string errorMessage) {
    // Checks that lookahead token is in allowedSet
      if (allowedSet.Contains(sym.kind)) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Mod2Decl() {}

  ++++++ */

    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

  //  To test the scanner we can use a loop like the following:

      do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym);

  /*  After the scanner is debugged we shall substitute this code:

      GetSym();                                   // Lookahead symbol
      Mod2Decl();                                 // Start to parse from the goal symbol
      // if we get back here everything must have been satisfactory
      Console.WriteLine("Parsed correctly");

  */
      output.Close();
    } // Main

  } // Declarations
