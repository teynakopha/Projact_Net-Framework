# Projact_Net-Framework
#--------------------การทำงานหน้า form ทำหน้า save config อย่างเดียว -------------------------------------
1.การทำงานของโปรแกรม จะทำการเชื่อมต่อไปยัง database vc ที่กำหนดไว้รองรับเฉพาะ vCenter ที่เป็น MSSQL เท่านั้น 
จะมีการ save config ip,user,password vCenter เข้าไปใน sqlite db 
#------------------------service --------------------------------------------- 
1.จะมีการเข้าไปโหลด การทำงานของ config ที่อยู่บน sql table scheduler 
2.ถ้า field auto_run = true จะทำการ run ลบ event บน vc by user ที่เรา config ตอนเริ่มใช้งานโปรแกรมจะ ไม่มีการ enable autorun ไว้
;
