Taken from https://people.csail.mit.edu/jaffer/r5rs_9.html

This section describes how individual tokens (identifiers, numbers, etc.) are formed from sequences of characters. The following sections describe how expressions and programs are formed from sequences of tokens.

<Intertoken space> may occur on either side of any token, but not within a token.

Tokens which require implicit termination (identifiers, numbers, characters, and dot) may be terminated by any <delimiter>, but not necessarily by anything else.

The following five characters are reserved for future extensions to the language: `[ ] { } |`

```
<token> --> <identifier> | <boolean> | <number>
     | <character> | <string>
     | ( | ) | #( | ' | ` | , | ,@ | .
<delimiter> --> <whitespace> | ( | ) | " | ;
<whitespace> --> <space or newline>
<comment> --> ;  <all subsequent characters up to a
                 line break>
<atmosphere> --> <whitespace> | <comment>
<intertoken space> --> <atmosphere>*

<identifier> --> <initial> <subsequent>*
     | <peculiar identifier>
<initial> --> <letter> | <special initial>
<letter> --> a | b | c | ... | z

<special initial> --> ! | $ | % | & | * | / | : | < | =
     | > | ? | ^ | _ | ~
<subsequent> --> <initial> | <digit>
     | <special subsequent>
<digit> --> 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9
<special subsequent> --> + | - | . | @
<peculiar identifier> --> + | - | ...
<syntactic keyword> --> <expression keyword>
     | else | => | define 
     | unquote | unquote-splicing
<expression keyword> --> quote | lambda | if
     | set! | begin | cond | and | or | case
     | let | let* | letrec | do | delay
     | quasiquote

`<variable> => <'any <identifier> that isn't
                also a <syntactic keyword>>

<boolean> --> #t | #f
<character> --> #\ <any character>
     | #\ <character name>
<character name> --> space | newline

<string> --> " <string element>* "
<string element> --> <any character other than " or \>
     | \" | \\ 

<number> --> <num 2>| <num 8>
     | <num 10>| <num 16>
```


The following rules for `<num R>`, `<complex R>`, `<real R>`, `<ureal R>`, `<uinteger R>`, and `<prefix R>` should be replicated for R = 2, 8, 10, and 16. There are no rules for `<decimal 2>`, `<decimal 8>`, and `<decimal 16>`, which means that numbers containing decimal points or exponents must be in decimal radix.

```
<num R> --> <prefix R> <complex R>
<complex R> --> <real R> | <real R> @ <real R>
    | <real R> + <ureal R> i | <real R> - <ureal R> i
    | <real R> + i | <real R> - i
    | + <ureal R> i | - <ureal R> i | + i | - i
<real R> --> <sign> <ureal R>
<ureal R> --> <uinteger R>
    | <uinteger R> / <uinteger R>
    | <decimal R>
<decimal 10> --> <uinteger 10> <suffix>
    | . <digit 10>+ #* <suffix>
    | <digit 10>+ . <digit 10>* #* <suffix>
    | <digit 10>+ #+ . #* <suffix>
<uinteger R> --> <digit R>+ #*
<prefix R> --> <radix R> <exactness>
    | <exactness> <radix R>


<suffix> --> <empty> 
    | <exponent marker> <sign> <digit 10>+
<exponent marker> --> e | s | f | d | l
<sign> --> <empty>  | + |  -
<exactness> --> <empty> | #i | #e
<radix 2> --> #b
<radix 8> --> #o
<radix 10> --> <empty> | #d
<radix 16> --> #x
<digit 2> --> 0 | 1
<digit 8> --> 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
<digit 10> --> <digit>
<digit 16> --> <digit 10> | a | b | c | d | e | f 
```