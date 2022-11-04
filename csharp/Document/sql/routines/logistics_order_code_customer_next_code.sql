DROP PROCEDURE IF EXISTS logistics_order_code_customer_next_code;

DELIMITER $

CREATE PROCEDURE logistics_order_code_customer_next_code(IN p_customer_id INT)
BEGIN
    DECLARE v_now DATETIME;
    DECLARE v_seq INT;

    SET v_now = NOW();

    SET v_seq = NULL;
    SELECT t.seq
    INTO v_seq
    FROM tbl_logistics_order_code_customer t
    WHERE t.customer_id = p_customer_id;

    IF v_seq IS NULL THEN
        INSERT INTO tbl_logistics_order_code_customer(customer_id, seq, create_time, update_time)
        VALUES (p_customer_id, 1, v_now, v_now);
        SET v_seq = 1;
        SELECT v_seq; # 这里不能简单的写成 SELECT 1; 否则客户端的会认为这是 Int64，而不是 Int32
    ELSE
        SET v_seq = v_seq + 1;

        UPDATE tbl_logistics_order_code_customer t
        SET t.seq         = v_seq,
            t.update_time = v_now
        WHERE t.customer_id = p_customer_id;

        SELECT v_seq;
    END IF;

END $


DELIMITER ;
