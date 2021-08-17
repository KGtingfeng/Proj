using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infor  
{
     
}

public class ResponseInfo<T>
{
    public T body;
    public Head head;
    public Page page;
}

[Serializable]
public class Head
{
    public string message;
    public int status;
}

[Serializable]
public class Page
{
    public int index;
    public int size;
}


[Serializable]
public class BookEntity
{
    public int id;
    public string title;
    public string author;
}

[Serializable]
public class BookInfo
{
    public List<BookEntity> bookList;
}