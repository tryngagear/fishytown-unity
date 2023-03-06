using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeWriter : MonoBehaviour
{
    [SerializeField] private float textSpeed = 50f;
    
    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>(){
        {new HashSet<char>(){'.','!','?'}, 0.6f},
        {new HashSet<char>(){',',';'}, 0.3f},
    };
    private Stack<char> RTS; 
    public Coroutine Run(string textToType, TMP_Text textLabel){
        return StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel){
        float time_elapsed = 0;
        int charIndex = 0;
        while (charIndex < textToType.Length){
            int lastCharIndex = charIndex;
            time_elapsed += Time.deltaTime * textSpeed;

            charIndex = Mathf.FloorToInt(time_elapsed);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            for(int i = lastCharIndex; i < charIndex; i++){ //causes weird issues
                bool isLast = i >= textToType.Length-1;
                
                textLabel.text = textToType.Substring(0, i+1);
   
                if(IsPunc(textToType[i], out float waitTime) && !isLast){
                    yield return new WaitForSeconds(waitTime);
                }
            }
            //textLabel.text = textToType.Substring(0, charIndex);


            yield return null;
        }
    }

    private bool IsPunc(char character, out float waitTime){ //causes weird issues fix later
        foreach(KeyValuePair<HashSet<char>, float> puncCat in punctuations){
            if(puncCat.Key.Contains(character)){
                waitTime = puncCat.Value;
                return true;
            }
        }
        waitTime = 0f;
        return false;
    }
}
