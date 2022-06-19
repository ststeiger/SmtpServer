-- Table: public.messages_map_content

-- DROP TABLE IF EXISTS public.messages_map_content;

CREATE TABLE IF NOT EXISTS public.messages_map_content
(
     msg_uid uuid NOT NULL 
    ,msg_bytes bytea 
    ,msg_body character varying COLLATE pg_catalog."default" 
    ,CONSTRAINT pk_messages_map_content PRIMARY KEY (msg_uid) 
    ,CONSTRAINT fk_messages_map_content_messages FOREIGN KEY (msg_uid) 
        REFERENCES public.messages (msg_uid) MATCH SIMPLE 
        ON UPDATE NO ACTION 
        ON DELETE NO ACTION 
) TABLESPACE pg_default; 

ALTER TABLE IF EXISTS public.messages_map_content OWNER to smtp_verver_web_services; 
