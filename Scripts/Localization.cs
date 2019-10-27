using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
using System.Linq;
/*
Класс для управлния локализацией.

Для правильнйо работы, необходимо все текстовыеэлементы которые подлежат переводу закинуть в инспекторе в лист с текстами (elements, 14 строка), а потом вызвать ОДИН раз метод BuildDefaultLocale().
Потом скопировать созданный файл и сохранить с другим именем и в ручную менять занчения слов, если необходимо, то еще и размер шрифтов менять. 
*/
public class Localization : MonoBehaviour
{
    public bool create;
    public Text[] elements; // все текстовые элементы интерфейса, для которых предусмотрен перевод
    public string path = ""; // путь где будут все локали
    public bool isRussian; // русский ли язык?

    [HideInInspector] public int[] idList; // создается/обновляется вместе с шаблоном языка

    void Start()
    {
        path = Application.persistentDataPath + "/Locale";
        DefaultLocale(false); // установка локали по умолчанию, в данном случае английская версия по умолчанию, тк передаем false, т.е. isRussian = false
        GetID();
        if(create)BuildDefaultLocale(); // Надо вызвать один раз, после того, как в инспекторе все тексты указаны в списке, для генерации XML. Он создаст файл, нужно его скопировать, переименовать и в ручную менять значения копии, для перевода
    }

    // создание массива id значений, относительно текстовых элементов
    void GetID()
    {
        int i = 1;
        idList = new int[elements.Length];
        for (int j = 0; j < elements.Length; j++)
        {
            if (idList[j] == 0)
            {
                idList[j] = i;
                i++;
            }
        }
    }

    public void DefaultLocale(bool value)
    {
        isRussian = value;
    }
    public void SetLanguage(bool value)
    {
        isRussian = value; //Указываем русский ли язык нужен
        SetLocale(); //Вызываем метод, считывающий из файла перевод и меняющий занчения текстов на нужный
    }
    public void BuildDefaultLocale() // для редактора, создание/обновление локали по умолчанию
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        GetID();
        string file = path + "/Default.xml"; // имя стандартной локали
        int[] fontSize = new int[elements.Length]; //массив содержащий размер шрифта, тк часто слова которые в оригинале влезали в кнопку, могут не влезть или быть слишком маленькими после перевода
        string[] arr = new string[elements.Length];
        for (int i = 0; i < elements.Length; i++)
        {
            arr[i] = elements[i].text; // копируем все текстовые элементы
            fontSize[i] = elements[i].fontSize; // копируем шрифты текстовых элементов

        }

        string[] res_txt = arr.ToArray(); //Копируем массив с текстами в другой массив
        int[] res_id = idList.ToArray(); //Преобразуем лист с id в массив

        XmlElement elm;
        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("Locale");
        xmlDoc.AppendChild(rootNode);

        for (int i = 0; i < res_txt.Length; i++) // запись в файл
        {
            elm = xmlDoc.CreateElement("text"); //Создаем элемент с именем text
            elm.SetAttribute("id", res_id[i].ToString()); //Записываем в атрибут id значение из массива с id
            elm.SetAttribute("fontSize", fontSize[i].ToString()); //Записываем в атрибут fontSize значение из массива с размерами шрифтов
            elm.SetAttribute("value", res_txt[i]); //Записываем в атрибут value значение из массива с текстом
            rootNode.AppendChild(elm); //добавляем элемент к корневому
        }

        xmlDoc.Save(file);
        Debug.Log(this + " Создан фаил --> " + file);
    }

    int GetInt(string text)
    {
        int value;
        if (int.TryParse(text, out value)) return value;
        return 0;
    }

    void SetLocale() // чтение файла и замена текста
    {
        TextAsset textAsset = isRussian ? (TextAsset) Resources.Load("Locale/Russian") : (TextAsset) Resources.Load("Locale/Default"); //Если стоит русский язык, то загружаем файл с русской локализацией, иначе по умолчанию
        XmlDocument xmldoc = new XmlDocument ();
        xmldoc.LoadXml ( textAsset.text ); //грузим XML из файла
        XmlElement xRoot = xmldoc.DocumentElement;

        foreach(XmlElement xnode in xRoot) //Перечисляем все дочерние элементы корневого элемента 
        {
            if(xnode.Name=="text") //если имя элемента == text
            {
                ReplaceText(GetInt(xnode.GetAttribute("id")), xnode.GetAttribute("value"), GetInt(xnode.GetAttribute("fontSize"))); //вызываем метод меняющий текст в массиве текстов на нужный
            }
        }
    }

    void ReplaceText(int id, string text, int fontSize) // поиск и замена всех элементов, по ключу
    {
        for (int j = 0; j < idList.Length; j++)
        {
            if (idList[j] == id)
            {
                elements[j].text = text;
                elements[j].fontSize = fontSize;                
            }
        }
    }
}