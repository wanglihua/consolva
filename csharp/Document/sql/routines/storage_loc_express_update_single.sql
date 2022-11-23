DROP PROCEDURE IF EXISTS storage_loc_express_update_single;

DELIMITER $

CREATE PROCEDURE storage_loc_express_update_single(IN p_storage_loc_id INT)
BEGIN
    DELETE FROM tbl_storage_loc_express WHERE storage_loc_id = p_storage_loc_id;

    INSERT INTO tbl_storage_loc_express(storage_loc_id, shelf_id, express_id)
    SELECT sl.id       AS storage_loc_id,
           sl.shelf_id AS shelf_id,
           e.id        AS express_id
    FROM tbl_storage_loc sl
             INNER JOIN tbl_express e ON e.storage_loc_id = sl.id
    WHERE sl.id = p_storage_loc_id
      AND e.status_id IN (
                          1000, # ExpressStatus.WaitPick
                          2000, # ExpressStatus.Expect
                          3000, # ExpressStatus.CheckIn
                          4000, # ExpressStatus.WaitReturn
                          7000, # ExpressStatus.WaitOrder
                          8000, # ExpressStatus.Order
                          9000, # ExpressStatus.WaitPack
                          10000 # ExpressStatus.PartOffShelf

        );
END $


DELIMITER ;