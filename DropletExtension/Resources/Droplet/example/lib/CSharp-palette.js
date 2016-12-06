({mode: 'coffeescript',
modeOptions:
  {
  functions:
  {
		playSomething: { command: true, color: 'red'},
		bk: { command: true, color: 'blue'},
		sin: { command: false, value: true, color: 'green' }
  },
		categories:
	{
		conditionals: { color: 'purple' },
		loops: { color: 'green' },
		functions: { color: '#49e' }
	}
	},
	palette: [
{
  name: 'Palette category',
	color: 'blue',
	blocks: [
{
  block: 'for [1..3]\\n  \`\`',
	title: 'Repeat some code'
},
	{
  block: 'playSomething()',
	expansion: 'playSomething \\'arguments\\', 100, \\'too long to show\\''
},
		]
	}
]})