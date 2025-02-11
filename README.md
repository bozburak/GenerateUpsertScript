SS
![image](https://github.com/user-attachments/assets/59d0b5d3-830c-4b29-abf0-3dd950624e5e)



![image](https://github.com/user-attachments/assets/441df1f7-f43f-4d45-bce4-e56a64d04024)![image](https://github.com/user-attachments/assets/14cb6132-f916-486f-8bd0-43e40f51e8ba)# GenerateUpsertScript
csv to sql


![image](https://github.com/user-attachments/assets/753c7147-e3f4-471f-aee5-d17e609a71f0)

copy with headers 

paste excel and save csv
![image](https://github.com/user-attachments/assets/4aa9c920-2bc3-4728-acca-b6832af34a05)
 




1. aşama

SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'tablo ismi'
AND COLUMN_NAME IN (
    SELECT COLUMN_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'tablo ismi'
    AND COLUMN_NAME IS NOT NULL
) order by COLUMN_NAME;


koduyla hedef tablonun kolon isimleri listelenir


2. aşama 


hedef tablonun kolon isimlerine göre kaynak tabloya sql yazılır

örn:

select id, date, columnName1, columnName1 from TableName



3. aşama 

headerlarla beraber select sorgusu CSV formatında kaydedilir.


4. aşama 

Program açılır ve dosya verilere çalıştırılır.



**Önemli**

Dosyayı programın okuması için açık olmaması gerekmektedir.
