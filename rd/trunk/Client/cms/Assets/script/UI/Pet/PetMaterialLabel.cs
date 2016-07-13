using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PetMaterialLabel : MonoBehaviour {

    public Text currentCountLabel;
    public Text needCountLabel;
    public Text sliceLabel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ReloadData(int currentCount, int needCount , int maxCount)
    {
        currentCount = currentCount > maxCount ? maxCount : currentCount;

        if (currentCountLabel != null && needCountLabel != null && sliceLabel != null)
        {
            currentCountLabel.text = currentCount.ToString();
            needCountLabel.text = needCount.ToString();
        
            if (needCount > currentCount)
            {
                currentCountLabel.color = needCountLabel.color = sliceLabel.color = Color.red;
            }
            else
            {
                currentCountLabel.color = needCountLabel.color = sliceLabel.color = Color.black;
            }
        }
    }
}
