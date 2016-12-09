({
    mode: 'javascript',
    modeOptions: {
      functions: {
                    },
        },
    palette: [
      {
        name: 'Variables',
        color: 'orange',
        blocks: [
          { block: 'var x = 0'},
        ]
      },

      {
        name: 'Operators',
        color: 'yellow',
        blocks: [
          { block: 'a + b' },
          { block: 'a - b' },
          { block: 'a * b' },
          { block: 'a / b' },
          { block: 'a % b' },
          { block: 'a++' },
          { block: 'a--' },

          { block: 'a = b'},
          { block: 'a += b'},
          { block: 'a -= b'},
          { block: 'a *= b'},
          { block: 'a /= b'},
          { block: 'a %= b'},


          { block: 'a == b' },
          { block: 'a === b' },
          { block: 'a != b' },
          { block: 'a > b' },
          { block: 'a >= b' },
          { block: 'a < b' },
          { block: 'a <= b' },

          { block: 'a && b' },
          { block: 'a || b' },
          { block: '!a' },

          { block: 'a & b' },
          { block: 'a | b' },
          { block: 'a ^ b' },
          { block: '~a' },
          { block: 'a << b' },
          { block: 'a >> b' },
          { block: '(a > b) ? 1 : 2'},

          { block: 'typeof a'},

          { block: 'true' },
          { block: 'false' }
        ]
      },

      {
        name: 'Controls',
        color: 'green',
        blocks: [
          { block: 'if (a == b){\\n\\ta += 1;\\n}'},
          { block: 'while (a == b){\\n\\ta += 1;\\n}'},
          { block: 'for (i = 0; i < 10; i++){\\n\\ta += b;\\n}'},
          { block: 'for (a in b){\\n\\ta += b;\\n}'},
        ]
      },

      {
        name: 'Functions',
        color: 'blue',
        blocks: [
          { block: 'function FunctionName(args){\\n\\treturn\\n}'},
          { block: 'FunctionName(args)'},
          { block: 'return value'},
          { block: 'return'},

        ]
      },

      {
        name: 'Classes',
        color: 'purple',
        blocks: [
         { block: 'class ClassName {\\n\\tconstructor(a, b) {\\n\\t\\tthis.a = a;\\nthis.b = b;\\n\\t}\\n}'},
        ]
      }
    ]
  })