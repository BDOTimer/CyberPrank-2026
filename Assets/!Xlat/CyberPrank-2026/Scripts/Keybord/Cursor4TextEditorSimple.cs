using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CP2026
{
    public class Cursor4TextEditorSimple : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI textTMP;
        [SerializeField] private float blinkInterval = 0.5f;
        [SerializeField] private Color cursorColor   = Color.white;
        [SerializeField] private float scaleY        = 0.945f;

        private LineRenderer lineRenderer;
        private int          cursorPosition = 0;
        private bool         isActive       = false;
        private Coroutine    blinkCoroutine;
        private Coroutine    updatePositionCoroutine;

        PositionsCursorOfNewLines positionsCursorOfNewLines;

        void Awake()
        {
            positionsCursorOfNewLines = new PositionsCursorOfNewLines();
            positionsCursorOfNewLines.Setup(textTMP);
        }

        void Start()
        {
            CreateLineCursor();
            StartCursor();
            SetCursorPosition2End();

            //Test();
        }
        
        private void StartCursor()
        {
            if (blinkCoroutine          != null) StopCoroutine(blinkCoroutine);
            if (updatePositionCoroutine != null) StopCoroutine(updatePositionCoroutine);
            
            blinkCoroutine          = StartCoroutine(BlinkCursor());
            updatePositionCoroutine = StartCoroutine(UpdateCursorPosition());
            
            if (lineRenderer != null)
                lineRenderer.enabled = true;
        }
        
        private void StopCursor()
        {
            if (blinkCoroutine          != null) StopCoroutine(blinkCoroutine);
            if (updatePositionCoroutine != null) StopCoroutine(updatePositionCoroutine);
            
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }

        //void Update()
        //{
        //    if (!isActive) return;
            
        //    // Обработка нажатия Enter
        //    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        //    {
        //        InsertNewLine();
        //    }
            
        //    // Обработка Backspace
        //    if (Input.GetKeyDown(KeyCode.Backspace))
        //    {
        //        DeleteCharacter();
        //    }
            
        //    // Обработка обычных символов
        //    if (Input.inputString.Length > 0)
        //    {
        //        foreach (char c in Input.inputString)
        //        {
        //            if (c == '\b') continue; // Backspace уже обработан
        //            if (c == '\n' || c == '\r') continue; // Enter уже обработан
        //            if (char.IsControl(c)) continue; // Другие управляющие символы
                    
        //            InsertCharacter(c);
        //        }
        //    }
            
        //    // Стрелки для навигации
        //    if (Input.GetKeyDown(KeyCode.LeftArrow))
        //        MoveCursorLeft();
        //    if (Input.GetKeyDown(KeyCode.RightArrow))
        //        MoveCursorRight();
        //    if (Input.GetKeyDown(KeyCode.Home))
        //        MoveCursorToStart();
        //    if (Input.GetKeyDown(KeyCode.End))
        //        MoveCursorToEnd();
        //}

        void CreateLineCursor()
        {
            GameObject lineGO = new GameObject("TextCursor");
            lineGO.transform.SetParent(transform);
            lineRenderer = lineGO.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.008f;
            lineRenderer.endWidth   = 0.008f;
            lineRenderer.material   = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = cursorColor;
            lineRenderer.endColor   = cursorColor;
        }

        IEnumerator BlinkCursor()
        {
            while (true)
            {
                if (lineRenderer != null)
                    lineRenderer.enabled = !lineRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
            }
        }

        IEnumerator UpdateCursorPosition()
        {
            while (true)
            {
                if (lineRenderer != null && textTMP != null && isActive)
                {
                    Vector3 cursorPos = GetCursorPosition();

                    float cursorHeight   = 0.03f;
                    Vector3 cursorTop    = cursorPos + Vector3.up * cursorHeight;
                    Vector3 cursorBottom = cursorPos + Vector3.down * 0.002f;
                    
                    lineRenderer.SetPosition(0, cursorTop);
                    lineRenderer.SetPosition(1, cursorBottom);
                }
                yield return null;
            }
        }

        // === МЕТОДЫ ДЛЯ РЕДАКТИРОВАНИЯ ===
        
        void InsertCharacter(char character)
        {
            string newText = textTMP.text.Insert(cursorPosition, character.ToString());
            textTMP.text = newText;
            cursorPosition++;
            textTMP.ForceMeshUpdate();
        }
        
        void InsertNewLine()
        {
            string newText = textTMP.text.Insert(cursorPosition, "\n");
            textTMP.text = newText;
            cursorPosition++;
            textTMP.ForceMeshUpdate();
            
            Debug.Log($"Enter pressed. New cursor position: {cursorPosition}");
        }
        
        void DeleteCharacter()
        {
            if (cursorPosition > 0)
            {
                string newText = textTMP.text.Remove(cursorPosition - 1, 1);
                textTMP.text = newText;
                cursorPosition--;
            }
        }
        
        void MoveCursorLeft()
        {
            if (cursorPosition > 0)
            {
                cursorPosition--;
                Debug.Log($"Cursor left: {cursorPosition}");
            }
        }
        
        void MoveCursorRight()
        {
            if (cursorPosition < textTMP.text.Length)
            {
                cursorPosition++;
                Debug.Log($"Cursor right: {cursorPosition}");
            }
        }
        
        void MoveCursorToStart()
        {
            cursorPosition = 0;
        }
        
        void MoveCursorToEnd()
        {
            cursorPosition = textTMP.text.Length;
        }

        // === МЕТОДЫ ПОЗИЦИОНИРОВАНИЯ КУРСОРА ===
        
        Vector3 GetCursorPosition()///////////////////////////////////////////////////////////////
        {
            textTMP.ForceMeshUpdate();

            if(textTMP.text == "")
            {   return GetCursorPositionStart();
            }

            if(textTMP.text[textTMP.text.Length - 1] == '\n')
            {   return GetCursorPositionStart(textTMP.textInfo.lineCount -1);
            }

            if (cursorPosition >= textTMP.text.Length)
            {
                return GetPositionAtTextEnd();
            }
            
            if (cursorPosition < textTMP.text.Length)
            {   char currentChar = textTMP.text[cursorPosition];
                if ( currentChar == '\n')
                {
                    return GetPositionAfterNewline(cursorPosition);
                }
            }
            
            return GetPositionAfterCharacter(cursorPosition);
        }
        
        Vector3 GetPositionAfterCharacter(int charIndex)
        {
            int tmpCharIndex = FindTMPCharacterIndex(charIndex);
            
            if (tmpCharIndex >= 0 && tmpCharIndex < textTMP.textInfo.characterCount)
            {
                TMP_CharacterInfo charInfo = textTMP.textInfo.characterInfo[tmpCharIndex];
            /// Vector3 pos = charInfo.bottomRight;
                Vector3 pos = charInfo.topRight;
                return textTMP.transform.TransformPoint(pos);
            }
            
            return GetPositionAtTextEnd();
        }
        
        int FindTMPCharacterIndex(int textIndex)
        {
            int tmpIndex = 0;
            for (int i = 0; i < textTMP.textInfo.characterCount && tmpIndex <= textIndex; i++)
            {
                TMP_CharacterInfo info = textTMP.textInfo.characterInfo[i];
                if (info.character != '\n')
                {
                    if (tmpIndex == textIndex)
                        return i;
                    tmpIndex++;
                }
                else
                {
                    if (tmpIndex == textIndex)
                        return i;
                    tmpIndex++;
                }
            }
            return -1;
        }
        
        Vector3 GetPositionAfterNewline(int newlineIndex)
        {
            for (int i = 0; i < textTMP.textInfo.lineCount; i++)
            {
                TMP_LineInfo line = textTMP.textInfo.lineInfo[i];
                
                if (line.firstCharacterIndex > newlineIndex)
                {
                    if (line.firstCharacterIndex < textTMP.textInfo.characterCount)
                    {
                        TMP_CharacterInfo firstChar = textTMP.textInfo.characterInfo[line.firstCharacterIndex];
                        Vector3 pos = firstChar.topLeft;
                        return textTMP.transform.TransformPoint(pos);
                    }
                }
            }
            
            return GetPositionAtTextEnd();
        }
        
        Vector3 GetPositionAtTextEnd() ///////////////////////////////////////////////////////////
        {
            int charCount = textTMP.textInfo.characterCount;
            if (charCount == 0)
                return textTMP.transform.position;

            ///////////////////////////////////////////
            bool isN = textTMP.text[cursorPosition-1] == '\n';
            ///////////////////////////////////////////

            TMP_LineInfo lastLine = textTMP.textInfo.lineInfo[textTMP.textInfo.lineCount - 1];
            
            int lastCharIndex = lastLine.lastCharacterIndex;
            
            if (lastCharIndex >= 0 && lastCharIndex < charCount)
            {
                TMP_CharacterInfo lastChar = textTMP.textInfo.characterInfo[lastCharIndex];
                Vector3 pos    = lastChar.bottomRight;

                Vector3 pos1   = textTMP.transform.TransformPoint(pos);
                if(isN) pos1.z = textTMP.transform.position.z;

                //Debug.Log($"isN: {isN}");

                return pos1;
            }
            
            if (lastLine.firstCharacterIndex < charCount)
            {
                TMP_CharacterInfo firstChar = textTMP.textInfo.characterInfo[lastLine.firstCharacterIndex];
                Vector3 pos = firstChar.topLeft;
                return textTMP.transform.TransformPoint(pos);
            }
            
            return textTMP.transform.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            On();
        }
        
        private void OnDestroy()
        {
            StopCursor();
        }
        
        private void OnDisable()
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
        }
        
        private void OnEnable()
        {
            if (isActive && lineRenderer != null)
                StartCursor();
        }
        
        // Для отладки
        //void OnDrawGizmos()
        //{
        //    if (textTMP == null || Application.isPlaying == false) return;
            
        //    textTMP.ForceMeshUpdate();
            
        //    for (int i = 0; i < textTMP.textInfo.lineCount; i++)
        //    {
        //        TMP_LineInfo line = textTMP.textInfo.lineInfo[i];
                
        //        if (line.firstCharacterIndex < textTMP.textInfo.characterCount)
        //        {
        //            Vector3 startPos = textTMP.transform.TransformPoint(
        //                textTMP.textInfo.characterInfo[line.firstCharacterIndex].topLeft
        //            );
        //            Gizmos.color = Color.green;
        //            Gizmos.DrawWireSphere(startPos, 0.01f);
        //        }
        //    }
        //}

        // === ПУБЛИЧНЫЕ МЕТОДЫ ВКЛЮЧЕНИЯ/ВЫКЛЮЧЕНИЯ ===
        
        public void On()
        {
            if (isActive) return;
            
            isActive = true;
            StartCursor();
            Debug.Log("Cursor enabled");
        }
        
        public void Off()
        {
            if (!isActive) return;
            
            isActive = false;
            StopCursor();
            Debug.Log("Cursor disabled");
        }

        public void SetCursorPosition(int position)
        {
            cursorPosition = Mathf.Clamp(position, 0, textTMP.text.Length);
            textTMP.ForceMeshUpdate();
        }

        public void SetCursorPosition2End()
        {
            cursorPosition = textTMP.text.Length;
            SetCursorPosition(cursorPosition);
        }

        ///////////////////////////////////////////////////////////////////////

        Vector3 GetCursorPositionStart(int lineCount = 0)
        {    
            // Получаем углы текста в мировых координатах
            Vector3[] corners = new Vector3[4];

            //textTMP.rectTransform.GetWorldCorners(corners);

            textTMP.rectTransform.GetLocalCorners(corners);

            corners[1].y -= GetLineSpacingSimple() + lineCount * 
                            textTMP.font.faceInfo.lineHeight * scaleY;

            return textTMP.transform.TransformPoint(corners[1]);
        }

        float GetLineSpacingSimple()
        {
            // Базовая высота строки из шрифта
            float  baseLineHeight = textTMP.font.faceInfo.lineHeight * 0.5f;
            return baseLineHeight;
        }

        Vector3 GetPositionNewLine() /////////////////////////////////////////////////////////////
        {
            int lineCount = textTMP.textInfo.lineCount;
            
            return GetCursorPositionStart(lineCount);
        }
    }


    ///-----------------------------------------------------------------------|
    /// Предвычисленные ЛОКАЛЬНЫЕ позиции курсора в начале N строк.
    ///-----------------------------------------------------------------------:
    public class PositionsCursorOfNewLines
    {
        Vector3  posStart;
        float LineSpacing;

        public void Setup(TextMeshProUGUI textTMP)
        {
            string mem = textTMP.text;

            textTMP.text = "a\na\n";
            textTMP.ForceMeshUpdate();

            posStart    = GetLocalPositionOfFirstCharacterPrecise(textTMP, 0);
            LineSpacing = GetLocalPositionOfFirstCharacterPrecise(textTMP, 1).y
                        - posStart.y;

            textTMP.text = mem;
        }

        public Vector3 Get(int numberLine)
        {
            Vector3 pos = posStart;

            posStart.y += numberLine * LineSpacing;

            return pos;
        }

        Vector3 GetLocalPositionOfFirstCharacterPrecise(TextMeshProUGUI textTMP, int lineIndex)
        {            
            TMP_LineInfo targetLine = textTMP.textInfo.lineInfo[lineIndex];
            int firstCharIndex = targetLine.firstCharacterIndex;
    
            return textTMP.textInfo.characterInfo[firstCharIndex].bottomLeft;
        }
    }
}