-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';



-- -----------------------------------------------------
-- Schema publshing_house_nosql
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `publshing_house_nosql` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `publshing_house_nosql` ;

-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`address_book`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`address_book` (
  `address_id` INT NOT NULL AUTO_INCREMENT,
  `country` VARCHAR(50) NOT NULL,
  `city` VARCHAR(30) NOT NULL,
  `street` VARCHAR(50) NOT NULL,
  `house` INT NOT NULL,
  `apartment` INT NULL DEFAULT NULL,
  PRIMARY KEY (`address_id`))
ENGINE = InnoDB
AUTO_INCREMENT = 351
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`person`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`person` (
  `id` INT NOT NULL,
  `name` VARCHAR(60) NOT NULL,
  `surname` VARCHAR(100) NOT NULL,
  `email` VARCHAR(150) NULL DEFAULT NULL,
  `phone_number` VARCHAR(12) NOT NULL,
  `address_book_address_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_person_address_book1_idx` (`address_book_address_id` ASC) VISIBLE,
  CONSTRAINT `fk_person_address_book1`
    FOREIGN KEY (`address_book_address_id`)
    REFERENCES `publshing_house_nosql`.`address_book` (`address_id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`author`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`author` (
  `pseudonym` VARCHAR(150) NULL DEFAULT NULL,
  `id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_author_person1_idx` (`id` ASC) VISIBLE,
  CONSTRAINT `fk_author_person1`
    FOREIGN KEY (`id`)
    REFERENCES `publshing_house_nosql`.`person` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`customer_type`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`customer_type` (
  `id` INT NOT NULL,
  `type` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`customer`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`customer` (
  `id` INT NOT NULL,
  `customer_type_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_customer_customer_type1_idx` (`customer_type_id` ASC) VISIBLE,
  INDEX `fk_customer_person1_idx` (`id` ASC) VISIBLE,
  CONSTRAINT `fk_customer_customer_type1`
    FOREIGN KEY (`customer_type_id`)
    REFERENCES `publshing_house_nosql`.`customer_type` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_customer_person1`
    FOREIGN KEY (`id`)
    REFERENCES `publshing_house_nosql`.`person` (`id`)
    ON DELETE CASCADE
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`genre`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`genre` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `genre` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `genre` (`genre` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 11
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`order_status`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`order_status` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `order_status` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `type_of_order` (`order_status` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 6
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`printing_house`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`printing_house` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(30) NOT NULL,
  `phone_number` VARCHAR(12) NOT NULL,
  `address` INT NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `name` (`name` ASC) VISIBLE,
  INDEX `fk_address_book_printing_house1` (`address` ASC) VISIBLE,
  CONSTRAINT `fk_address_book_PrHouse`
    FOREIGN KEY (`address`)
    REFERENCES `publshing_house_nosql`.`address_book` (`address_id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
AUTO_INCREMENT = 21
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`order`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`order` (
  `id` INT NOT NULL,
  `acceptance_date` DATETIME NOT NULL,
  `status` INT NOT NULL,
  `printing_house` INT NOT NULL,
  `total_price` DECIMAL(12,2) NOT NULL,
  `customer` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_order_order_status_idx` (`status` ASC) VISIBLE,
  INDEX `fk_order_printing_house1_idx` (`printing_house` ASC) VISIBLE,
  INDEX `fk_customer` (`customer` ASC) VISIBLE,
  CONSTRAINT `fk_customer`
    FOREIGN KEY (`customer`)
    REFERENCES `publshing_house_nosql`.`customer` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_order_order_status`
    FOREIGN KEY (`status`)
    REFERENCES `publshing_house_nosql`.`order_status` (`id`),
  CONSTRAINT `fk_order_printing_house2`
    FOREIGN KEY (`printing_house`)
    REFERENCES `publshing_house_nosql`.`printing_house` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`publication`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`publication` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(255) NOT NULL,
  `page_count` INT NOT NULL,
  `circulation` INT NOT NULL,
  `genre_id` INT NOT NULL,
  `price` DECIMAL(7,2) NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_publication_genre1_idx` (`genre_id` ASC) VISIBLE,
  CONSTRAINT `fk_publication_genre1`
    FOREIGN KEY (`genre_id`)
    REFERENCES `publshing_house_nosql`.`genre` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
AUTO_INCREMENT = 118
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`print_quality`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`print_quality` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `cost_per_sheet` INT NOT NULL,
  `quality` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `cost_per_sheet` (`cost_per_sheet` ASC) VISIBLE,
  UNIQUE INDEX `quality` (`quality` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`order_publication`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`order_publication` (
  `order` INT NOT NULL,
  `punlication` INT NOT NULL,
  `print_quality` INT NOT NULL,
  `quantity` INT NOT NULL,
  PRIMARY KEY (`order`, `punlication`),
  INDEX `fk_order_has_publications_publications1_idx` (`punlication` ASC) VISIBLE,
  INDEX `fk_order_has_publications_order1_idx` (`order` ASC) VISIBLE,
  INDEX `fk_order_publication_print_quality1_idx` (`print_quality` ASC) VISIBLE,
  CONSTRAINT `fk_order_has_publications_order1`
    FOREIGN KEY (`order`)
    REFERENCES `publshing_house_nosql`.`order` (`id`)
    ON DELETE CASCADE
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_order_has_publications_publications1`
    FOREIGN KEY (`punlication`)
    REFERENCES `publshing_house_nosql`.`publication` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_order_publication_print_quality1`
    FOREIGN KEY (`print_quality`)
    REFERENCES `publshing_house_nosql`.`print_quality` (`id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `publshing_house_nosql`.`publication_author`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `publshing_house_nosql`.`publication_author` (
  `publications_id` INT NOT NULL,
  `author_id` INT NOT NULL,
  PRIMARY KEY (`publications_id`, `author_id`),
  INDEX `fk_author_has_publications_publications1_idx` (`publications_id` ASC) VISIBLE,
  INDEX `fk_publication_author_author1_idx` (`author_id` ASC) VISIBLE,
  CONSTRAINT `fk_author_has_publications_publications1`
    FOREIGN KEY (`publications_id`)
    REFERENCES `publshing_house_nosql`.`publication` (`id`)
    ON DELETE CASCADE
    ON UPDATE RESTRICT,
  CONSTRAINT `fk_publication_author_author1`
    FOREIGN KEY (`author_id`)
    REFERENCES `publshing_house_nosql`.`author` (`id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;




INSERT INTO `publshing_house_nosql`.`order_status` (id,`order_status`) VALUES
    (1,'Pending'),      -- Order has been placed and is awaiting processing
    (2,'InProduction'), -- Order is being prepared (e.g., books are being printed)
    (3,'Shipped'),      -- Order has been shipped to the customer
    (4,'Delivered'),    -- Order has been delivered to the customer
    (5,'Canceled'); 

    INSERT INTO customer_type (id, customer_type.type)
VALUES
(1, 'Retailer'),
(2, 'Distributor'),
(3, 'DirectCustomer'),
(4, 'OnlineStore'),
(5, 'Author');

INSERT INTO `publshing_house_nosql`.`print_quality` (id,`quality`, `cost_per_sheet`) VALUES
    (1, 'Low', 5),        
    (2, 'Medium', 10),   
    (3, 'High', 15),      
    (4, 'Premium', 25);

    INSERT INTO genre (id,genre) VALUES 
    (1,'Fiction'),
    (2,'Non-Fiction'),
    (3,'Fantasy'),
    (4,'Science Fiction'),
    (5,'Mystery'),
    (6,'Biography'),
    (7,'History'),
    (8,'Children'),
    (9,'Poetry'),
    (10,'Graphic Novels');