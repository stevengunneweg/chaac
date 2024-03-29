using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellManager : MonoBehaviour {

    private Object[] _spellsObjects;
    private Object[] _elementsObjects;
    private Spell[] _spells;
    private Element[] _elements;

    private List<Element> selectedElems = new List<Element>();
    private string _currentSelectedElementName = "";
    
    public float maxDistance = 50f;

    // Use this for initialization
    void Start () {
        SetSpellAndElements();
    }

    public Spell FindSpell(List<Element> elements){
        
        foreach(Spell spell in _spells){
            
            if(spell.Elements.Count != elements.Count){
                continue;
            }
            bool spellAvailable = true;
            foreach(Element el in elements)
            {
                if (!el.Available)
                    spellAvailable = false;

            }
            if (!spellAvailable)
                continue;
            bool hasElements = elements.All(el => spell.Elements.Contains(el));
            if(hasElements){
                
                    return spell;
            }
        }

        return null;
    }

    internal void AddElementToQueue(Element value)
    {
		Sound sound = new Sound (transform.root.gameObject.GetComponent<AudioSource> (), "SFX/" + "Click");
        if (PlayerManager.FreeMode)
        {
            if (_currentSelectedElementName == value.Name)
                return;
        }
            selectedElems.Clear();
        selectedElems.Add(value);

        _currentSelectedElementName = value.Name;
    }
    
    void SetSpellAndElements()
    {
        //_spells = Resources.FindObjectsOfTypeAll<Spell>();
        //_elements = Resources.FindObjectsOfTypeAll<Element>();
        _spellsObjects = Resources.LoadAll("Data", typeof(Spell));
        _elementsObjects = Resources.LoadAll("Data", typeof(Element));
        _spells = new Spell[_spellsObjects.Length];
        _elements = new Element[_elementsObjects.Length];
        for (int i=0;i< _spellsObjects.Length;i++)
        {
            _spells[i] = _spellsObjects[i] as Spell;
        }
        for (int i = 0; i < _elementsObjects.Length; i++)
        {
            _elements[i] = _elementsObjects[i] as Element;
            _elements[i].ResetCoolDown();
            _elements[i].Free = PlayerManager.FreeMode;
        }
    }

    public Spell[] Spells
    {
        get { return _spells; }
    }
    public Element[] Elements
    {
        get { return _elements; }
    }
    void Update()
    {
        foreach (Element el in Elements)
        {
            el.Update();
        }
    }

    void FixedUpdate() {
        //if mouse button (left hand side) pressed instantiate a raycast
        if (Input.GetMouseButtonDown(0)) {
            //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance)) {
                //log hit area to the console
                Spell cur_spell = FindSpell(selectedElems);

                if (cur_spell != null) {

                    if (!PlayerManager.FreeMode)
                        FindObjectOfType<TabPanel>().DeselectAll();

                    //Show spell effect
                    GameObject effectGameObject = Instantiate(cur_spell.Effect);

                    effectGameObject.transform.position += hit.point;
                    foreach (Element el in selectedElems)
                    {
                        el.StartCooldown();
                    }

					if (cur_spell.name == "EarthSpell") {
						//Sound sound = new Sound (transform.root.gameObject.GetComponent<AudioSource> (), "SFX/" + "WilhelmScream");
					}

					if (cur_spell.name == "WindSpell") {
						Sound sound = new Sound (transform.root.gameObject.GetComponent<AudioSource> (), "SFX/" + "Wind");
					}

					if (cur_spell.name == "FireSpell") {
						Sound sound = new Sound (transform.root.gameObject.GetComponent<AudioSource> (), "SFX/" + "Meteor");
					}

					if (cur_spell.name == "LightningSpell") {
						Sound sound = new Sound (transform.root.gameObject.GetComponent<AudioSource> (), "SFX/" + "Lightning");
					}


					Debug.Log (cur_spell.name);

                    //Check collision with enemies
                    Collider[] hitColliders = Physics.OverlapSphere(hit.point, cur_spell.Radius);
                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        Enemy enemy = hitColliders[i].GetComponent<Enemy>();
                        if (enemy != null)
						{
							effectGameObject.GetComponent<BaseSpellEffect>().ApplyEffectToEnemy(enemy);
                        }
                    }
                    if (!PlayerManager.FreeMode)
                        selectedElems.Clear();
                }
            }
        }
    }
    public List<Element> SelectedElements()
    {
        return selectedElems;
    }
}
