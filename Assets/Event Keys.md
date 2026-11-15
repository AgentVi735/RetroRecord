**Local Events**

* Lane Switch = LS00000000

  * lane setting (0,1,2,3) default is 00001111



**Global Events**

* Show Song Info = SI

  * active for 2/1 beats
* Effect = Ef-X (x == event key)

  * Flame effect = Fl

    * active for 1/4 beat
  * Spark effect = Sp

    * active for 1/2 beat
  * Smoke effect = Sm

    * active for 1/4 beat
* Light = Li-X (X == Lights\[] index \&\& A == Type)

  * Types

    * Static = LiS\_A-X
    * Flashing = LiF\_A-X
    * Lerp = LiL\_A-X
  * Times

    * 1/1 = Li\_W-X
    * 1/2 = Li\_H-X
    * 1/4 = Li\_Q-X
    * 1/8 = Li\_V-X
    * 1/16 = Li\_S-X
    * 1/32 = Li\_T-X
    * 1/64 = Li\_F-X

