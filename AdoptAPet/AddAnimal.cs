﻿using AdoptAPet.HelperClasses;
using AdoptAPet.HelperFunctions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace AdoptAPet
{
    public partial class AddAnimal : Form
    {
        public AddAnimal()
        {
            InitializeComponent();
            cbSex.SelectedIndex = 0;
            populateComboboxes();
        }

        /// <summary>
        /// Populate the initial comboboxes, setting the data which will automatically populate the rest of the information.
        /// </summary>
        public void populateComboboxes()
        {
            var speciesBox = Queries.returnAllSpeciesName();

            cbSpecies.Items.Add("Select a Species");

            foreach (var item in speciesBox)
            {
                cbSpecies.Items.Add(item);
            }

            cbSpecies.SelectedIndex = 0;
        }

        private void cbSpecies_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            cbBreed.Items.Clear();

            var breedBox = Queries.returnBreedBySpecies(cbSpecies.SelectedItem.ToString());

            cbBreed.Items.Add("Select a Breed");
            cbBreed.SelectedIndex = 0;

            foreach (var item in breedBox)
            {
                cbBreed.Items.Add(item.ToString());
            }
        }

        private void ofdAddAnimal_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnBrowseIMage_Click(object sender, EventArgs e)
        {
            ofdAddAnimal.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            var userClickedOK = ofdAddAnimal.ShowDialog();

            if(File.Exists(ofdAddAnimal.FileName))
            {
                pbPreview.ImageLocation = ofdAddAnimal.FileName;
            }

            else
            {

            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Animal toAdd = null;

            bool continueOn = true;
            int ImageId = 0;

            try
            {
                XDocument doc = Imgur.uploadImage(ofdAddAnimal.FileName);
                string link = doc.Root.Element("link").Value;
                string deletehash = doc.Root.Element("deletehash").Value;
                ImageId = Queries.addImageToDatabase(link, deletehash);
            }

            catch(Exception ex)
            {
                continueOn = false;
                MessageBox.Show("Something messed up with imgur!");
            }



            try
            {
                if (continueOn)
                {
                    int t_age = Int32.Parse(txtAge.Text);
                    string t_description = rtbDescription.Text;
                    bool t_friendly = cbFriendly.Checked;
                    string t_name = txtName.Text;
                    string t_sex = cbSex.SelectedItem.ToString();
                    bool t_isFixed = cbFixed.Checked;

                    toAdd = new Animal()
                    {
                        age = t_age,
                        description = t_description,
                        friendly = t_friendly,
                        name = t_name,
                        sex = t_sex,
                        isFixed = t_isFixed,
                        imgid = ImageId
                    };

                    string species = cbSpecies.SelectedItem.ToString();
                    string breed = cbBreed.SelectedItem.ToString();

                    Queries.addAnimal(species, breed, toAdd);

                    MessageBox.Show("Animal Added Successfully!");

                }
            }

            catch(Exception ex)
            {
                MessageBox.Show("Incorrect input format");
            }
        }
    }
}
