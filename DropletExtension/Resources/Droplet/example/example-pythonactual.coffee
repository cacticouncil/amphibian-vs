# Droplet config editor
window.expressionContext = {
  prefix: 'a = '
}

window.dropletConfig = ace.edit 'droplet-config'
dropletConfig.setTheme 'ace/theme/chrome'
dropletConfig.getSession().setMode 'ace/mode/python'

# dropletConfig.setValue localStorage.getItem('config') ? '''
dropletConfig.setValue '''
  ({
    'mode': 'python',
    'modeOptions': {
      'functions': {

        'newLine': {
          'color': 'blue' }
       }
     },

    'palette': [
      {
        'name': 'Imports',
        'color': 'green',
        'blocks': [

          { 'block': 'import library_name' },

          { 'block': 'from library_name import library_package' }

        ]
      },

      {
        'name': 'Text',
        'color': 'blue',
        'blocks': [

          { 'block': 'print "Hello, World!"' },

          { 'block': 'input("Enter a number: ")' },

          { 'block': 'raw_input("What is your name? ")' },

          { 'block': 'len(string_variable)' },

          { 'block': "newLine()" }

        ]
      },

      {
        'name': 'Control',
        'color': 'orange',
        'blocks': [

          { 'block': 'if a == b:\\n  print "This is a conditional if statement!"' },

          { 'block': 'while a == b:\\n  print "This is a conditional loop!"' },

          { 'block': 'for i in list_variable:\\n  print list_variable[i]' },

          { 'block': 'break' },

          { 'block': 'continue' },

          { 'block': 'pass' }

        ]
      },

      {
        'name': 'Functions',
        'color': 'purple',
        'blocks': [

          { 'block': 'def FunctionName(args):\\n  return' },

          { 'block': 'FunctionName(args)' },

          { 'block': 'lambda_variable = lambda args: args * 2' },

          { 'block': 'return return_value' },

          { 'block': 'return' }

        ]
      },

      {
        'name': 'Operations',
        'color': 'teal',
        'blocks': [
          {
            'block': 'a = 1'
          },
          {
            'block': 'a == b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a < b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a > b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a and b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a or b',
            'wrapperContext': expressionContext
          },

          {
            'block': 'a + b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a - b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a * b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a / b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a % b',
            'wrapperContext': expressionContext
          },
          {
            'block': 'a ** b',
            'wrapperContext': expressionContext
          }
        ]
      }
    ]
  })
'''

# Droplet itself
createEditor = (options) ->
  $('#droplet-editor').html ''
  editor = new droplet.Editor $('#droplet-editor')[0], options

  editor.setEditorState localStorage.getItem('blocks') is 'yes'
  editor.aceEditor.getSession().setUseWrapMode true

  # Initialize to starting text
  editor.setValue localStorage.getItem('text') ? ''

  editor.on 'change', ->
    localStorage.setItem 'text', editor.getValue()

  window.editor = editor

createEditor eval dropletConfig.getValue()

$('#toggle').on 'click', ->
  editor.toggleBlocks()
  localStorage.setItem 'blocks', (if editor.currentlyUsingBlocks then 'yes' else 'no')

# Stuff for testing convenience
$('#update').on 'click', ->
  localStorage.setItem 'config', dropletConfig.getValue()
  createEditor eval dropletConfig.getValue()

configCurrentlyOut = localStorage.getItem('configOut') is 'yes'

updateConfigDrawerState = ->
  if configCurrentlyOut
    $('#left-panel').css 'left', '0px'
    $('#right-panel').css 'left', '525px'
  else
    $('#left-panel').css 'left', '-500px'
    $('#right-panel').css 'left', '25px'

  editor.resize()

  localStorage.setItem 'configOut', (if configCurrentlyOut then 'yes' else 'no')

$('#close').on 'click', ->
  configCurrentlyOut = not configCurrentlyOut
  updateConfigDrawerState()

updateConfigDrawerState()