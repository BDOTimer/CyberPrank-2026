var SkulptLib = {
    InitSkulpt: function() {
        console.log("Unity вызывает InitSkulpt");
        
        if (typeof Sk !== 'undefined') {
            var self = this;
            
            Sk.configure({
                output: function(text) {
                    // Получаем доступ к экземпляру Unity
                    if (typeof unityInstance !== 'undefined') {
                        unityInstance.SendMessage('PythonExecutor', 'OnPythonOutput', text);
                    } else if (typeof Module !== 'undefined' && Module.SendMessage) {
                        Module.SendMessage('PythonExecutor', 'OnPythonOutput', text);
                    } else {
                        console.error("Unity instance не найдена!");
                    }
                },
                read: function(filename) {
                    if (Sk.builtinFiles === undefined || Sk.builtinFiles.files[filename] === undefined) {
                        return null;
                    }
                    return Sk.builtinFiles.files[filename];
                },
                __future__: Sk.python3
            });
            console.log("Skulpt сконфигурирован");
        } else {
            console.error("Skulpt не загружен!");
        }
    },

    RunPython: function(codePtr) {
        var code = UTF8ToString(codePtr);
        console.log("Unity отправляет код:", code);
        
        var promise = Sk.misceval.asyncToPromise(function() {
            return Sk.importMainWithBody('<stdin>', false, code, false);
        });
        
        promise.then(function(mod) {
            console.log("Код выполнен успешно");
            if (typeof unityInstance !== 'undefined') {
                unityInstance.SendMessage('PythonExecutor', 'OnPythonSuccess', 'Done');
            }
        }).catch(function(err) {
            console.error("Ошибка:", err);
            if (typeof unityInstance !== 'undefined') {
                unityInstance.SendMessage('PythonExecutor', 'OnPythonError', err.toString());
            }
        });
    }
};

mergeInto(LibraryManager.library, SkulptLib);