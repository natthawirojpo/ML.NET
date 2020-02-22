Accessing specific indices with IEnumerable

If you only need access to a portion of the data or specific indices, 
use CreateEnumerable and set the reuseRowObject parameter value to false 
so a new object is created for each of the requested rows in the dataset. 
Then, convert the IEnumerable to an array or list.

Once the collection has been created, you can perform operations on the data. 
The code snippet below takes the first three rows in the dataset 
and calculates the average current price.