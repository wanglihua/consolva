DROP PROCEDURE IF EXISTS storage_loc_express_update_all;

DELIMITER $

# 里面有事务处理
CREATE PROCEDURE storage_loc_express_update_all()
BEGIN

    DECLARE v_storage_loc_id INT;

    DECLARE done INT DEFAULT FALSE;
    DECLARE v_has_error BOOL DEFAULT FALSE;


    DECLARE cursor_storage_loc_id CURSOR FOR
        SELECT sl.id
        FROM tbl_storage_loc sl
                 INNER JOIN tbl_shelf s ON sl.shelf_id = s.id
        WHERE s.status_id = 1000
          AND sl.status_id = 1000;

    DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;

    DECLARE CONTINUE HANDLER FOR SQLEXCEPTION SET v_has_error = TRUE;

    OPEN cursor_storage_loc_id;

    loop_storage_loc_id:
    LOOP
        FETCH cursor_storage_loc_id INTO v_storage_loc_id;

        IF done THEN
            LEAVE loop_storage_loc_id;
        END IF;

        # 开启事务
        START TRANSACTION;
        CALL storage_loc_express_update_single(v_storage_loc_id);
        IF v_has_error THEN
            ROLLBACK;
        ELSE
            COMMIT;
        END IF;

    END LOOP;

    CLOSE cursor_storage_loc_id;

END $


DELIMITER ;