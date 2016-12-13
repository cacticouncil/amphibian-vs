({
    mode: 'c_cpp',
    modeOptions: {
      functions: {

       }
     },

    palette: [
      {
        name: 'Libraries',
        color: 'red',
        blocks: [
          { block: '#include <iostream>' },
          { block: 'using namespace std;' },
          { block: '#include <vector>' },
          { block: '#include <string>' },
        ]
      },

      {
        name: 'Functions',
        color: 'blue',
        blocks: [
          { block: 'void main()\\n{\\n\\tint x = 1;\\n}' },
          { block: 'void myFunction(void)\\n{\\n\\tint x = 1;\\n}' },
          { block: 'myFunction();' }

        ]
      },

      {
        name: 'Operators',
        color: 'yellow',
        blocks: [
          { block: 'int x;' },
          { block: 'x = x + 1;' },
          { block: 'x = x - 1;' },
          { block: 'x = x * 1;' },
          { block: 'x = x / 1;' },
        ]
      },

      {
        name: 'Controls',
        color: 'green',
        blocks: [

        ]
      },

      {
        name: 'Classes',
        color: 'purple',
        blocks: [

        ]
      }
    ]
  })