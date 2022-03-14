# RS2SwaggerAPI
C# wrapper for the RS2 Swagger REST API. This takes advantage of dynamics and can return etheir an array or a list of dynamic objects. 


CREATE
CreateDefault - Returns a blank (empty/null values) for the selected table
Update the needed values and then use Create to create the new record. A copy of the new record will be returned

SelectAll - Will return all records for the given table

Select (ID) will return the single record for that ID value

Select - with filter - wil return all records that match the filter criteria

Update - get the record, make changes to the values, send the record back
