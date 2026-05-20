mergeInto(LibraryManager.library, {
  InitSkulpt: function() {
    console.log('InitSkulpt called');
    if (typeof Sk !== 'undefined') {
      Sk.configure({
        output: function(text) {
          console.log('Python output:', text);
          Module.SendMessage('PythonExecutor', 'OnPythonOutput', text);
        },
        read: function(filename) {
          if (Sk.builtinFiles === undefined || Sk.builtinFiles.files[filename] === undefined) {
            return null;
          }
          return Sk.builtinFiles.files[filename];
        },
        __future__: Sk.python3
      });
      console.log('Skulpt configured');
    } else {
      console.error('Skulpt not loaded');
    }
  },
  
  RunPython: function(codePtr) {
    console.log('RunPython called');
    var code = UTF8ToString(codePtr);
    console.log('Code to execute:', code);
    
    Sk.misceval.asyncToPromise(function() {
      return Sk.importMainWithBody('<stdin>', false, code, false);
    }).then(function(mod) {
      console.log('Python success');
      Module.SendMessage('PythonExecutor', 'OnPythonSuccess', 'Done');
    })['catch'](function(err) {
      console.error('Python error:', err);
      Module.SendMessage('PythonExecutor', 'OnPythonError', err.toString());
    });
  }
});