DROP PROCEDURE IF EXISTS checkin_tracking_no_next_seq;

DELIMITER $$

CREATE PROCEDURE checkin_tracking_no_next_seq()
BEGIN
    DECLARE v_now DATETIME;
    DECLARE v_year INT;
    DECLARE v_month INT;
    DECLARE v_seq INT;

    SET v_now = NOW();

    SET v_year = YEAR(v_now);
    SET v_month = MONTH(v_now);

    SET v_seq = NULL;
    SELECT t.seq
    INTO v_seq
    FROM tbl_checkin_tracking_no t
    WHERE t.year = v_year
      AND t.month = v_month;

    IF v_seq IS NULL THEN
        INSERT INTO tbl_checkin_tracking_no (year, month, seq, create_time, update_time)
        VALUES (v_year, v_month, 1, v_now, v_now);

        SELECT v_year AS year, v_month AS month, 1 AS seq;
    ELSE
        SET v_seq = v_seq + 1;

        UPDATE tbl_checkin_tracking_no t
        SET t.seq         = v_seq,
            t.update_time = v_now
        WHERE t.year = v_year
          AND t.month = v_month;

        SELECT v_year, v_month, v_seq;
    END IF;

END $$

DELIMITER ;
