using UnityEngine;
using UnityEngine.UI;

public class ChangingInstructionsBehaviour : MonoBehaviour
{
    [SerializeField] Sprite[] _instructions;
    [SerializeField] Button _previousButton;
    [SerializeField] Button _nextButton;

    Image _image;
    int _currentInstruction;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable() // everytime the instruction screens appears
    {
        // reset image to starter
        _currentInstruction = 0;
        _image.sprite = _instructions[_currentInstruction];

        // set buttons
        _previousButton.gameObject.SetActive(false);

        if (_instructions.Length > 1)
        {
            _nextButton.gameObject.SetActive(true);
        }
        else
        {
            _nextButton.gameObject.SetActive(false);
        }

        // set listeners
        _previousButton.onClick.AddListener(PreviousInstruction);
        _nextButton.onClick.AddListener(NextInstruction);
    }

    private void OnDisable()
    {
        _previousButton.onClick.RemoveListener(PreviousInstruction);
        _nextButton.onClick.RemoveListener(NextInstruction);
    }

    private void PreviousInstruction()
    {
        _currentInstruction--;
        _image.sprite = _instructions[_currentInstruction];

        if (_currentInstruction == 0) // it has gone back to the first image
        {
            _previousButton.gameObject.SetActive(false); // hide the "previous button"
        } 
        else if (_currentInstruction == _instructions.Length - 2) // it has gone back to the second to last image
        {
            _nextButton.gameObject.SetActive(true); // show the "next button"
        }
    }
    
    private void NextInstruction()
    {
        _currentInstruction++;
        _image.sprite = _instructions[_currentInstruction];

        if (_currentInstruction == 1) // it has passed the first image
        {
            _previousButton.gameObject.SetActive(true); // show the "previous button"
        }
        else if (_currentInstruction == _instructions.Length - 1) // it's on the last image
        {
            _nextButton.gameObject.SetActive(false); // hide the "next button"
        }
    }
}