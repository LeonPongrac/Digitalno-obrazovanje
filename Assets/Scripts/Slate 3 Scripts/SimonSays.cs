using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSays : MonoBehaviour
{
    [SerializeField]
    public StartGameConfig startGameConfig;
    
    [System.Serializable]
    public class LightCube
    {
        public GameObject cube; // Kocka koja sadr�i Light komponentu i Renderer
        public Material highlightMaterial;
        [HideInInspector]
        public Material defaultMaterial;

        public void InitializeDefaultMaterial()
        {
            if (cube != null)
            {
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    defaultMaterial = renderer.material;
                }
            }
        }
    }
    
    public LightCube[] lights;
    public GameObject startButton;
    public GameObject feedbackLight;
    public GameObject endGameLight;
    public Material correctMaterial; // Zeleni materijal za ispravan odgovor
    public Material incorrectMaterial; // Crveni materijal za netocan odgovor
    public Material endGameLightSuccessMaterial; // Materijal za "endGameLight" kada svijetli zeleno
    [Range(0, 3)]
    private int[] correctAnswers; // Lista tocnih odgovora za svaku sekvencu (1 = prvo svjetlo, 2 = drugo itd.)

    private List<int[]> allSequences; // Automatski generirane sekvence
    private int currentSequenceIndex; // Indeks trenutne sekvence u bazenu
    private float speedMultiplier = 1f; // Faktor ubrzanja sekvence (1 = normalna brzina)
    private int correctLightIndex; // Tocan odgovor za trenutnu sekvencu
    private bool gameRunning = false; // Sprjecava visestruko pokretanje
    public bool GameRunning => gameRunning;
    private bool sequenceFinished = false; // Kontrola za dozvolu klika na svjetla
    public bool SequenceFinished => sequenceFinished;
    private bool answerGiven = true; // Kontrola da je odgovor vec odabran
    public bool AnswerGiven => answerGiven;
    private Material originalFeedbackMaterial; // Pamti originalni materijal povratnog svjetla
    private Material endGameLightMaterial; // Pamti originalni materijal za endGameLight
    private int correctAnswersCount = 0; // Brojac tocnih odgovora
    private bool successAchieved = false; // Status postignuca (5 tocnih odgovora)

    
    void Start()
    {
        // Generiraj sve permutacije
        // allSequences = GeneratePermutations(new[] { 0, 1, 2, 3 });

        // Zamijeniti generiranje permutacija s eksplicitnom listom
        allSequences = new List<int[]>
        {
            new int[] { 1, 0, 2, 3 }, // red,blue,yellow,green -> red (1)
            new int[] { 1, 0, 3, 2 }, // red,blue,green,yellow -> blue (0)
            new int[] { 1, 2, 0, 3 }, // red,yellow,blue,green -> red (1)
            new int[] { 1, 2, 3, 0 }, // red,yellow,green,blue -> yellow (2)
            new int[] { 1, 3, 0, 2 }, // red,green,blue,yellow -> green (3)
            new int[] { 1, 3, 2, 0 }, // red,green,yellow,blue -> blue (0)
            new int[] { 0, 1, 2, 3 }, // blue,red,yellow,green -> green (3)
            new int[] { 0, 1, 3, 2 }, // blue,red,green,yellow -> blue (0)
            new int[] { 0, 2, 1, 3 }, // blue,yellow,red,green -> yellow (2)
            new int[] { 0, 2, 3, 1 }, // blue,yellow,green,red -> red (1)
            new int[] { 0, 3, 1, 2 }, // blue,green,red,yellow -> green (3)
            new int[] { 0, 3, 2, 1 }, // blue,green,yellow,red -> blue (0)
            new int[] { 2, 1, 0, 3 }, // yellow,red,blue,green -> red (1)
            new int[] { 2, 1, 3, 0 }, // yellow,red,green,blue -> yellow (2)
            new int[] { 2, 0, 1, 3 }, // yellow,blue,red,green -> green (3)
            new int[] { 2, 0, 3, 1 }, // yellow,blue,green,red -> blue (0)
            new int[] { 2, 3, 1, 0 }, // yellow,green,red,blue -> red (1)
            new int[] { 2, 3, 0, 1 }, // yellow,green,blue,red -> green (3)
            new int[] { 3, 1, 0, 2 }, // green,red,blue,yellow -> green (3)
            new int[] { 3, 1, 2, 0 }, // green,red,yellow,blue -> yellow (2)
            new int[] { 3, 0, 1, 2 }, // green,blue,red,yellow -> blue (0)
            new int[] { 3, 0, 2, 1 }, // green,blue,yellow,red -> green (3)
            new int[] { 3, 2, 1, 0 }, // green,yellow,red,blue -> yellow (2)
            new int[] { 3, 2, 0, 1 }  // green,yellow,blue,red -> red (1)
        };

        // Točni odgovori koji odgovaraju gornjim permutacijama
        correctAnswers = new int[] 
        {
            1, 0, 1, 2, 3, 0, 3, 0, 2, 1, 3, 0, 1, 2, 3, 0, 1, 3, 3, 2, 0, 3, 2, 1
        };

        // Provjeri je li broj odgovora tocno 24
        if (correctAnswers.Length != allSequences.Count)
        {
            Debug.LogError($"Broj odgovora ({correctAnswers.Length}) mora biti tocno {allSequences.Count}.");
            return;
        }

        // Inicijalizacija svih default materijala za svjetla
        foreach (var lightCube in lights)
        {
            lightCube.InitializeDefaultMaterial();
        }

        // Pohranjivanje originalnog materijala povratnog svjetla
        Renderer feedbackRenderer = feedbackLight.GetComponent<Renderer>();
        if (feedbackRenderer != null)
        {
            originalFeedbackMaterial = feedbackRenderer.material;
        }

        // Pohranjivanje originalnog materijala za endGameLight
        /*
        Renderer successRenderer = endGameLight.GetComponent<Renderer>();
        if (successRenderer != null)
        {
            endGameLightMaterial = successRenderer.material;
        }
        */
    }

    void Update()
    {
        // Provjera klika na StartButton ili na jedno od 4 svjetla
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == startButton && !gameRunning && !sequenceFinished && answerGiven && !successAchieved)
                {
                    StartGameSequence();
                }
                else if (sequenceFinished)
                {
                    CheckUserClick(hit.transform.gameObject);
                }
            }
        }
    }

    void StartGameSequence()
    {
        gameRunning = true;
        answerGiven = false;

        // Odabir nasumicne sekvence (permutacije) iz trenutnog bazena sekvenci
        int randomIndex = Random.Range(0, allSequences.Count);
        int[] chosenSequence = allSequences[randomIndex];
        correctLightIndex = correctAnswers[randomIndex]; // Postavi tocan odgovor za ovu sekvencu

        Debug.Log($"Chosen Sequence: {string.Join(",", chosenSequence)}");
        Debug.Log($"Correct Answer (Light Index): {correctLightIndex}");

        // Pokretanje sekvence
        StartCoroutine(PlayLightSequence(chosenSequence));

        // Spremi indeks za uklanjanje nakon odgovora ako se odabrao tocan odgovor
        currentSequenceIndex = randomIndex;
    }


    IEnumerator PlayLightSequence(int[] sequence)
    {
        sequenceFinished = false;

        foreach (int index in sequence)
        {
            LightCube lightCube = lights[index];

            lightCube.cube.GetComponent<Renderer>().material = lightCube.highlightMaterial;
            lightCube.cube.GetComponent<Light>().enabled = true;

            // Cekaj ovisno o faktoru ubrzanja
            yield return new WaitForSeconds(1f / speedMultiplier);

            lightCube.cube.GetComponent<Renderer>().material = lightCube.defaultMaterial; // Vra�anje na originalni crni materijal
            lightCube.cube.GetComponent<Light>().enabled = false;

            // Pauza izmedu bljeskanja
            yield return new WaitForSeconds(0.5f / speedMultiplier);
        }

        sequenceFinished = true;
    }


    void CheckUserClick(GameObject clickedObject)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            if (clickedObject == lights[i].cube)
            {
                if (i == correctLightIndex)
                {
                    Debug.Log("Correct");
                    correctAnswersCount++;

                    // Ukloni trenutnu sekvencu i pripadajuci odgovor iz bazena sekvenci
                    allSequences.RemoveAt(currentSequenceIndex);
                    correctAnswers = RemoveAtIndex(correctAnswers, currentSequenceIndex);

                    // Povecaj faktor brzine
                    speedMultiplier = speedMultiplier + 0.75f;

                    CheckSuccess();
                    StartCoroutine(ShowFeedback(true));
                }
                else
                {
                    Debug.Log("Incorrect");
                    StartCoroutine(ShowFeedback(false));
                    startGameConfig.AddStrike();
                }

                sequenceFinished = false;
                return;
            }
        }
    }

    IEnumerator ShowFeedback(bool isCorrect)
    {
        // Ukljucivanje svjetla za povratnu informaciju
        Light lightComponent = feedbackLight.GetComponent<Light>();
        Renderer renderer = feedbackLight.GetComponent<Renderer>();

        if (isCorrect)
        {
            lightComponent.color = Color.green;
            renderer.material = correctMaterial;
        }
        else
        {
            lightComponent.color = Color.red;
            renderer.material = incorrectMaterial;
        }

        lightComponent.enabled = true;

        // Drzi svjetlo ukljucenim dvije sekunde
        yield return new WaitForSeconds(2);

        // Iskljuci svjetlo i vrati originalni materijal
        lightComponent.enabled = false;
        renderer.material = originalFeedbackMaterial;
        answerGiven = true;
        gameRunning = false;
    }

    void CheckSuccess()
    {
        if (correctAnswersCount >= 5)
        {
            successAchieved = true;

            // Ukljucivanje svjetla za kraj igre
            Light endGameLightComponent = endGameLight.GetComponent<Light>();
            Renderer endGameLightRenderer = endGameLight.GetComponent<Renderer>();

            endGameLightComponent.color = Color.green;
            endGameLightRenderer.material = endGameLightSuccessMaterial;
            endGameLightComponent.enabled = true;
        }
    }

    private int[] RemoveAtIndex(int[] array, int index)
    {
        if (array == null || index < 0 || index >= array.Length)
        {
            return array; // Vracamo isti niz ako je index nevazeci
        }

        // Uklanjanje indeksa na koji se tocno odgorilo
        int[] newArray = new int[array.Length - 1];
        for (int i = 0, j = 0; i < array.Length; i++)
        {
            if (i == index) continue;
            newArray[j++] = array[i];
        }
        return newArray;
    }

    // Sve funkcije ispod nisu potrebne
    List<int[]> GeneratePermutations(int[] array)
    {
        var results = new List<int[]>();
        Permute(array, 0, array.Length - 1, results);
        return results;
    }

    void Permute(int[] array, int l, int r, List<int[]> results)
    {
        if (l == r)
        {
            results.Add((int[])array.Clone());
        }
        else
        {
            for (int i = l; i <= r; i++)
            {
                (array[l], array[i]) = (array[i], array[l]);
                Permute(array, l + 1, r, results);
                (array[l], array[i]) = (array[i], array[l]);
            }
        }
    }

}
