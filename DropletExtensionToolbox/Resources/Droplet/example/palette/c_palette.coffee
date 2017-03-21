({
    mode: 'c',
    modeOptions: {
      functions: {
                 
                 },
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
          { block: '#include "myClass.h"' },
        ]
      },

      {
        name: 'Functions',
        color: 'blue',
        blocks: [
          { block: 'int main()\\n{\\n\\t\\n}' },
          { block: 'void myFunction(void)\\n{\\n\\t\\n}' },
		  { block: 'void myFunction(void);' },

        ]
      },

      {
        name: 'Variables',
        color: 'green',
        blocks: [
          { block: 'int x;' },
          { block: 'int x[5];' },
          { block: 'int* x;' },



        ]
      },

      {
        name: 'Operators',
        color: 'yellow',
        blocks: [
		   { block: 'myFunction(b);' }
        ]
      },

      {
        name: 'Controls',
        color: 'orange',
        blocks: [
        ]
      },

      {
        name: 'Classes',
        color: 'purple',
        blocks: [

        ]
      },


      {
        name: 'Misc',
        color: 'black',
        blocks: [
          { block: '// this is a comment' },
          { block: '#define PI 3.14159' },
          { block: '#pragma region RegionName'},
          { block: '#pragma endregion OptionalComment'},

        ]
      },
    ]
})