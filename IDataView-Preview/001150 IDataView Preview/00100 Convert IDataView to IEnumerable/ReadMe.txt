Convert IDataView to IEnumerable

One of the quickest ways to inspect an IDataView is to convert it to an IEnumerable. 
To convert an IDataView to IEnumerable use the CreateEnumerable method.

To optimize performance, set reuseRowObject to true. Doing so will lazily populate 
the same object with the data of the current row as it's being evaluated as opposed 
to creating a new object for each row in the dataset.